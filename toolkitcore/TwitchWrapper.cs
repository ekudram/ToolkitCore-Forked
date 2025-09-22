/*
 * File: TwitchWrapper.cs
 * Project: ToolkitCore
 * 
 * Updated: September 20, 2025
 * 
 * Summary of Changes:
 * 1. Converted from static class to instance class to properly access instance-based settings
 * 2. Added reference to ToolkitCore mod instance to access settings
 * 3. Updated all method signatures to be instance methods instead of static
 * 4. Modified all settings references to use the instance-based approach
 * 5. Added error handling for null settings references
 * 6. Changed initial connection error from Error to Warning as requested
 * 7. Added static Client property for backward compatibility with Utilities mod
 * 8. Resolved naming conflicts between instance and static Client properties
 * 9. Fixed threading issues by using LongEventHandler for main thread operations
 * 10. Fixed ClientOptions compatibility with newer TwitchLib version
 * 11. Added proper rate limiting for message sending
 * 12. Replaced Task.Run with LongEventHandler for RimWorld compatibility
 * 13. Added message truncation to comply with Twitch's 500-character limit
 * 14. Added 1st-time chatter detection and welcome message
 *  
 * 
 * Why These Changes Were Made:
 * The TwitchWrapper class was previously accessing settings through static fields,
 * which conflicted with our changes to make settings instance-based for better mod reload safety.
 * Converting to an instance class allows proper access to the settings through the mod instance.
 * The change from Error to Warning for initial connection errors provides a better user experience
 * for first-time users who haven't configured their Twitch credentials yet.
 * The threading fixes prevent main thread assertion errors by ensuring game state operations
 * are performed on the main thread using RimWorld's LongEventHandler.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ToolkitCore.Controllers;
using ToolkitCore.Models;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;
using TwitchLib.Communication.Models;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Verse;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

namespace ToolkitCore
{
    public class TwitchWrapper
    {
        private readonly ToolkitCore _mod;
        private static TwitchWrapper Instance;
        private TwitchClient _client;
        private string GetInstructionsMessageViaReflection(string username)
        {
            try
            {

                if (!ToolkitCoreInitializer.TwitchToolkitLoaded)
                {
                    ToolkitCoreLogger.Debug("TwitchToolkit not loaded, using fallback message");
                    return GetFallbackInstructionsMessage(username);
                }
                // Check if TwitchToolkit is loaded by looking for its assembly
                Assembly twitchToolkitAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.FullName.Contains("TwitchToolkit"));

                if (twitchToolkitAssembly == null)
                {
                    ToolkitCoreLogger.Warning("TwitchToolkit assembly not found. Skipping welcome message.");
                    return null;
                }

                // Get the MessageHelpers type - note the corrected namespace
                Type messageHelpersType = twitchToolkitAssembly.GetType("TwitchToolkit.Utilities.MessageHelpers");
                if (messageHelpersType == null)
                {
                    ToolkitCoreLogger.Warning("MessageHelpers type not found in TwitchToolkit.");
                    return null;
                }

                // Get the GetInstructionsMessage method
                MethodInfo method = messageHelpersType.GetMethod("GetInstructionsMessage",
                    BindingFlags.Public | BindingFlags.Static);
                if (method == null)
                {
                    ToolkitCoreLogger.Warning("GetInstructionsMessage method not found.");
                    return null;
                }

                ToolkitCoreLogger.Debug($"Using TwitchToolkit assembly: {ToolkitCoreInitializer.TwitchToolkitAssembly.FullName}");
                // Invoke the method statically
                object result = method.Invoke(null, new object[] { username });
                return result?.ToString();
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error invoking GetInstructionsMessage via reflection: {ex.Message}");
                return GetFallbackInstructionsMessage(username);
            }
        }
        private string GetFallbackInstructionsMessage(string username)
        {
            return $"@{username} this is a mod where you earn coins while watching. Use !instructions for more info about available commands!";
        }


        // Public static property for backward compatibility
        public static TwitchClient Client => Instance?._client;

        // For rate limiting
        private DateTime _lastMessageTime = DateTime.MinValue;
        private readonly TimeSpan _messageDelay = TimeSpan.FromMilliseconds(100);

        public TwitchWrapper(ToolkitCore mod)
        {
            _mod = mod;
            Instance = this;
        }

        // Static methods for backward compatibility with external mods
        public static void StartAsync()
        {
            Instance?.StartAsyncInstance();
        }

        public static void SendChatMessage(string message)
        {
            Instance?.SendChatMessageInternal(message);
        }
        public static void OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            ToolkitCoreLogger.Error("Client has experienced a connection error. " + e.Error?.ToString());
        }

        public static void OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            ToolkitCoreLogger.Warning("Client has disconnected");
        }

        public static void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            ToolkitCoreLogger.Message("New Subscriber. " + e.Subscriber.DisplayName);
        }

        public static void OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            ToolkitCoreLogger.Message("Resub Subscriber. " + e.ReSubscriber.DisplayName);
        }

        public static void OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            ToolkitCoreLogger.Message("Being raided by " + e.RaidNotification.DisplayName);
        }

        public static void OnUserBanned(object sender, OnUserBannedArgs e)
        {
            ToolkitCoreLogger.Message("User has been banned - " + e.UserBan.Username);
        }
        public static void OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            ToolkitCoreLogger.Message("Gifted Subscription. " + e.GiftedSubscription.DisplayName);
        }
        public static void OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            ToolkitCoreLogger.Message($"User joined: {e.Username}");
            // Additional logic for mods to hook into

        }
        public static void OnUserLeft(object sender, OnUserLeftArgs e)
        {
            ToolkitCoreLogger.Message($"User left: {e.Username}");
            // Additional logic for mods to hook into
        }
        // Instance methods (implementation)
        public void StartAsyncInstance()
        {
            // Debug logging to help troubleshoot
            //ToolkitCoreLogger.Debug($"[ToolkitCore Debug] Token before validation: '{ToolkitCoreSettings.oauth_token}'");

            if (string.IsNullOrEmpty(ToolkitCoreSettings.oauth_token) ||
                string.IsNullOrEmpty(ToolkitCoreSettings.bot_username))
            {
                ToolkitCoreLogger.Error("  Missing Twitch credentials. Please check your settings.");
                return;
            }

            // Ensure token has the correct format
            string formattedToken = ToolkitCoreSettings.oauth_token;
            if (!formattedToken.StartsWith("oauth:"))
            {
                formattedToken = "oauth:" + formattedToken;
                ToolkitCoreLogger.Warning("  Auto-corrected token format by adding 'oauth:' prefix");

                // Update the settings with the corrected token
                ToolkitCoreSettings.oauth_token = formattedToken;
            }

            // Additional validation - check token length
            if (formattedToken.Length < 30) // oauth: + at least 24 characters
            {
                ToolkitCoreLogger.Error("  Token appears to be too short. Please check your token.");
                return;
            }

            // ToolkitCoreLogger.Debug($"[ToolkitCore Debug] Token after validation: '{formattedToken}'");

            Initialize(new ConnectionCredentials(ToolkitCoreSettings.bot_username, formattedToken));
        }
        public void Initialize(ConnectionCredentials credentials)
        {
            ResetClient();
            InitializeClient(credentials);
        }
        // Resets the Twitch client to a new instance
        private void ResetClient()
        {
            try
            {
                UnsubscribeFromEvents();
                if (_client != null && _client.IsConnected)
                    _client.Disconnect();

                _client = new TwitchClient(new WebSocketClient(new ClientOptions()));

            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Warning($"Error resetting Twitch client: {ex.Message}");
                if (ex.InnerException != null)
                {
                    ToolkitCoreLogger.Warning($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        private void UnsubscribeFromEvents()
        {
            // Unsubscribe from all events to prevent memory leaks
            if (_client == null) return;

            _client.OnConnected -= OnConnected;
            _client.OnJoinedChannel -= OnJoinedChannel;
            // ... unsubscribe from all other events
        }

        private void InitializeClient(ConnectionCredentials credentials)
        {
            if (_client == null)
            {
                ToolkitCoreLogger.Error("Tried to initialize null client, report to mod author");
                return;
            }

            try
            {
                // Initialize with credentials and channel
                _client.Initialize(credentials, ToolkitCoreSettings.channel_username);

                // Subscribe to events
                _client.OnConnected += OnConnected;
                _client.OnJoinedChannel += OnJoinedChannel;
                _client.OnMessageReceived += OnMessageReceived;
                _client.OnWhisperReceived += OnWhisperReceived;
                _client.OnWhisperCommandReceived += OnWhisperCommandReceived;
                _client.OnChatCommandReceived += OnChatCommandReceived;
                _client.OnConnectionError += OnConnectionError;
                _client.OnDisconnected += OnDisconnected;
                _client.OnNewSubscriber += OnNewSubscriber;
                _client.OnReSubscriber += OnReSubscriber;
                _client.OnRaidNotification += OnRaidNotification;
                _client.OnUserBanned += OnUserBanned;

                //_client.OnMessageThrottled += OnMessageThrottled;
                _client.OnChatCleared += OnChatCleared;
                _client.OnUserJoined += OnUserJoined;
                _client.OnUserLeft += OnUserLeft;

                // Connect to Twitch
                _client.Connect();
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"  Error initializing Twitch client: {ex.Message}");
                // Handle reconnection or notify user
            }
        }

        private void OnChatCleared(object sender, OnChatClearedArgs e)
        {
            ToolkitCoreLogger.Message("Chat was cleared");
        }

        private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            // Queue whisper processing for main thread
            LongEventHandler.QueueLongEvent(() =>
            {
                ProcessWhisperOnMainThread(e.WhisperMessage);
            }, "ProcessTwitchWhisper", false, null);
        }

        private void ProcessWhisperOnMainThread(WhisperMessage message)
        {
            try
            {
                if (Current.Game == null || !ToolkitCoreSettings.allowWhispers)
                    return;

                foreach (var twitchInterfaceBase in Current.Game.components.OfType<TwitchInterfaceBase>().ToList())
                    twitchInterfaceBase.ParseWhisper(message);

                MessageLog.LogMessage(message);
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"  Error processing whisper: {ex.Message}");
            }
        }

        private void OnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
        {
            // Queue whisper command processing for main thread
            LongEventHandler.QueueLongEvent(() =>
            {
                ProcessWhisperCommandOnMainThread(e.Command);
            }, "ProcessTwitchWhisperCommand", false, null);
        }

        private void ProcessWhisperCommandOnMainThread(WhisperCommand command)
        {
            try
            {
                if (Current.Game == null || !ToolkitCoreSettings.allowWhispers)
                    return;

                foreach (var twitchInterfaceBase in Current.Game.components.OfType<TwitchInterfaceBase>().ToList())
                    twitchInterfaceBase.ParseWhisperCommand(command);
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"  Error processing whisper command: {ex.Message}");
            }
        }

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            ToolkitCoreLogger.Message($"Connected to Twitch as {e.BotUsername}");
            // Connection established - no game state access needed
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            // todo: list.Twitch has a first time Chatter flag that could be implementied, to call this for them.Will ad to todo list
            // Use RimWorld's LongEventHandler instead of Task.Run for thread safety
            if (!ToolkitCoreSettings.sendMessageToChatOnStartup)
                return;

            LongEventHandler.QueueLongEvent(() =>
            {
                try
                {
                    _client.SendMessage(e.Channel, "Toolkit Core has Connected to Chat", false);
                }
                catch (Exception ex)
                {
                    ToolkitCoreLogger.Error($"  Error sending connection message: {ex.Message}");
                }
            }, "SendTwitchConnectionMessage", false, null);
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            // Queue message processing for main thread
            ToolkitCoreLogger.Debug($"Queueing message from {e.ChatMessage.Username}: {e.ChatMessage.Message}");
            LongEventHandler.QueueLongEvent(() =>
            {
                ProcessMessageOnMainThread(e.ChatMessage);
            }, "ProcessTwitchMessage", false, null);
        }

        private void ProcessMessageOnMainThread(ChatMessage message)
        {
            try
            {
                MessageLog.LogMessage(message);
                ToolkitCoreLogger.Debug($"Processing ProcessMessageOnMainThread from {message.Username}: {message.Message}");
                if (message.Bits > 0)
                    Log.Message("Bits donated : " + message.Bits.ToString());

                if (Current.Game == null)
                    return; // Game not loaded, skip processing 


                // Check if this is a first-time chatter
                if (message.IsFirstMessage)
                {
                    // Additional check to ensure TwitchToolkit is ready
                    if (Current.Game != null &&
                        Current.Game.components.OfType<TwitchInterfaceBase>().Any() &&
                        ToolkitCoreInitializer.TwitchToolkitLoaded)
                    {
                        string welcomeMessage = $"Welcome to the stream, @{message.Username}! ";
                        string instructions = GetInstructionsMessageViaReflection(message.Username);
                        welcomeMessage += instructions ?? GetFallbackInstructionsMessage(message.Username);

                        LongEventHandler.QueueLongEvent(() =>
                        {
                            SendChatMessageInternal(welcomeMessage);
                        }, "SendWelcomeMessage", false, null);
                    }
                }
                
                foreach (var twitchInterfaceBase in Current.Game.components.OfType<TwitchInterfaceBase>().ToList())
                {
                    twitchInterfaceBase.ParseMessage(message);
                }

            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error processing message: {ex.Message}");
            }
        }
        private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            // Queue command processing for main thread
            LongEventHandler.QueueLongEvent(() =>
            {
                ProcessChatCommandOnMainThread(e.Command);
            }, "ProcessTwitchCommand", false, null);
        }

        private void ProcessChatCommandOnMainThread(ChatCommand command)
        {
            try
            {
                if (Current.Game == null || ToolkitCoreSettings.forceWhispers)
                    return;

                return;
                // Duplicate processing, ignore
                        // Process all messages through ProcessMessageOnMainThread
                        // Doing the below causes double-processing of messages
                        //    // Add this to also process commands through TwitchInterfaceBase
                        //    if (command.ChatMessage != null)
                        //    {
                        //        foreach (var twitchInterfaceBase in Current.Game.components.OfType<TwitchInterfaceBase>().ToList())
                        //            twitchInterfaceBase.ParseMessage(command.ChatMessage);
                        //    }
                        //    else
                        //    {
                        //        ToolkitCoreLogger.Warning("  ChatCommand does not contain a ChatMessage reference");
                        //    }
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"  Error processing chat command: {ex.Message}");
            }
            
        }
        // In TwitchWrapper.cs, in the SendChatMessageInternal method:
        public void SendChatMessageInternal(string message)
        {
            if (_client == null || string.IsNullOrEmpty(ToolkitCoreSettings.channel_username))
                return;

            // Truncate message if it exceeds Twitch's 500-character limit
            if (message.Length > 500)
            {
                ToolkitCoreLogger.Warning($"Message truncated from {message.Length} to 500 characters");
                message = message.Substring(0, 497) + "...";
            }

            // Simple rate limiting
            var now = DateTime.Now;
            if (now - _lastMessageTime < _messageDelay)
            {
                Task.Delay(_messageDelay - (now - _lastMessageTime)).Wait();
            }

            _lastMessageTime = DateTime.Now;

            var channel = _client.GetJoinedChannel(ToolkitCoreSettings.channel_username);
            if (channel != null)
            {
                _client.SendMessage(channel, message, false);
            }
        }
    }
}
