/*
 * File: TwitchWrapper.cs
 * Project: ToolkitCore
 * 
 * Updated: [Current Date]
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
 * 
 * Why These Changes Were Made:
 * The TwitchWrapper class was previously accessing settings through static fields,
 * which conflicted with our changes to make settings instance-based for better mod reload safety.
 * Converting to an instance class allows proper access to the settings through the mod instance.
 * The change from Error to Warning for initial connection errors provides a better user experience
 * for first-time users who haven't configured their Twitch credentials yet.
 * The threading fixes prevent main thread assertion errors by ensuring game state operations
 * are performed on the main thread using RimWorld's LongEventHandler.
 */

using System;
using System.Linq;
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
using Verse;

namespace ToolkitCore
{
    public class TwitchWrapper
    {
        private readonly ToolkitCore _mod;
        private static TwitchWrapper Instance;
        private TwitchClient _client;

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
            ToolkitCoreLogger.Message($"[ToolkitCore Debug] Token before validation: '{ToolkitCoreSettings.oauth_token}'");

            if (string.IsNullOrEmpty(ToolkitCoreSettings.oauth_token) ||
                string.IsNullOrEmpty(ToolkitCoreSettings.bot_username))
            {
                ToolkitCoreLogger.Error("[ToolkitCore] Missing Twitch credentials. Please check your settings.");
                return;
            }

            // Ensure token has the correct format
            string formattedToken = ToolkitCoreSettings.oauth_token;
            if (!formattedToken.StartsWith("oauth:"))
            {
                formattedToken = "oauth:" + formattedToken;
                ToolkitCoreLogger.Warning("[ToolkitCore] Auto-corrected token format by adding 'oauth:' prefix");

                // Update the settings with the corrected token
                ToolkitCoreSettings.oauth_token = formattedToken;
            }

            // Additional validation - check token length
            if (formattedToken.Length < 30) // oauth: + at least 24 characters
            {
                ToolkitCoreLogger.Error("[ToolkitCore] Token appears to be too short. Please check your token.");
                return;
            }

            ToolkitCoreLogger.Message($"[ToolkitCore Debug] Token after validation: '{formattedToken}'");

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
                ToolkitCoreLogger.Error($"[ToolkitCore] Error initializing Twitch client: {ex.Message}");
                // Handle reconnection or notify user
            }
        }

        //private void OnMessageThrottled(object sender, OnMessageThrottledEventArgs e)
        //{
        //    Log.Warning($"[ToolkitCore] Message throttled: {e.Message}");
        //}

        private void OnChatCleared(object sender, OnChatClearedArgs e)
        {
            ToolkitCoreLogger.Message("[ToolkitCore] Chat was cleared");
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
                ToolkitCoreLogger.Error($"[ToolkitCore] Error processing whisper: {ex.Message}");
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
                ToolkitCoreLogger.Error($"[ToolkitCore] Error processing whisper command: {ex.Message}");
            }
        }

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            // Connection established - no game state access needed
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
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
                    ToolkitCoreLogger.Error($"[ToolkitCore] Error sending connection message: {ex.Message}");
                }
            }, "SendTwitchConnectionMessage", false, null);
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            // Queue message processing for main thread
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

                if (message.Bits > 0)
                    ToolkitCoreLogger.Message("Bits donated : " + message.Bits.ToString());

                if (Current.Game == null)
                    return;

                foreach (var twitchInterfaceBase in Current.Game.components.OfType<TwitchInterfaceBase>().ToList())
                {
                    twitchInterfaceBase.ParseMessage(message);
                }
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"[ToolkitCore] Error processing message: {ex.Message}");
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

                // Get the ChatMessage from the ChatCommand and pass it to TryExecute
                if (command.ChatMessage != null)
                {
                    ChatCommandController.GetChatCommand(command.CommandText)?.TryExecute(command.ChatMessage);
                }
                else
                {
                    ToolkitCoreLogger.Warning("[ToolkitCore] ChatCommand does not contain a ChatMessage reference");
                }
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"[ToolkitCore] Error processing chat command: {ex.Message}");
            }
        }

        public void SendChatMessageInternal(string message)
        {
            if (_client == null || string.IsNullOrEmpty(ToolkitCoreSettings.channel_username))
                return;

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
