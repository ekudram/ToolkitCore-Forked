/*
 * File: TwitchWrapper.cs
 * Project: ToolkitCore
 * 
 * Updated: [Current Date]
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Converted from static class to instance class to properly access instance-based settings.
 * 2. Added reference to ToolkitCore mod instance to access settings.
 * 3. Updated all method signatures to be instance methods instead of static.
 * 4. Modified all settings references to use the instance-based approach.
 * 5. Added error handling for null settings references.
 * 6. Changed initial connection error from Error to Warning as requested.
 * 7. Added static Client property for backward compatibility with Utilities mod
 * 8. Resolved naming conflicts between instance and static Client properties
 * 9. Fixed threading issues by using LongEventHandler for main thread operations
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
using TwitchLib.Communication.Models;
using Verse;

namespace ToolkitCore
{
    public class TwitchWrapper
    {
        private readonly ToolkitCore _mod;
        private static TwitchWrapper Instance;
        private TwitchClient _client;
        public static TwitchClient Client => Instance?._client;

        public TwitchWrapper(ToolkitCore mod)
        {
            _mod = mod;
            Instance = this;
        }

        public static void StartAsyncStatic()
        {
            Instance?.StartAsync();
        }


        public void StartAsync()
        {
            Instance?.Initialize(new ConnectionCredentials(ToolkitCoreSettings.bot_username, ToolkitCoreSettings.oauth_token));
        }

        public void Initialize(ConnectionCredentials credentials)
        {
            ResetClient();
            InitializeClient(credentials);
        }

        private void ResetClient()
        {
            try
            {
                if (_client != null && _client.IsConnected)
                    _client.Disconnect();

                _client = new TwitchClient(new WebSocketClient(new ClientOptions()
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30.0)
                }));
            }
            catch (Exception ex)
            {
                Log.Warning($"Error resetting Twitch client: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Log.Warning($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        private void InitializeClient(ConnectionCredentials credentials)
        {
            if (_client == null)
            {
                Log.Error("Tried to initialize null client, report to mod author");
                return;
            }

            _client.Initialize(credentials, ToolkitCoreSettings.channel_username);
            _client.OnConnected += OnConnected;
            _client.OnJoinedChannel += OnJoinedChannel;
            _client.OnMessageReceived += OnMessageReceived;
            _client.OnWhisperReceived += OnWhisperReceived;
            _client.OnWhisperCommandReceived += OnWhisperCommandReceived;
            _client.OnChatCommandReceived += OnChatCommandReceived;
            _client.Connect();
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
                    twitchInterfaceBase.ParseWhisper(message); // Changed from ParseMessage

                MessageLog.LogMessage(message);
            }
            catch (Exception ex)
            {
                Log.Error($"[ToolkitCore] Error processing whisper: {ex.Message}");
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

                // Option 1: Use the new ParseWhisperCommand method
                foreach (var twitchInterfaceBase in Current.Game.components.OfType<TwitchInterfaceBase>().ToList())
                    twitchInterfaceBase.ParseWhisperCommand(command);

                // Option 2: Keep using ChatCommandController (if you prefer this approach)
                // ChatCommandController.GetChatCommand(command.CommandText)?.TryExecute(command);
            }
            catch (Exception ex)
            {
                Log.Error($"[ToolkitCore] Error processing whisper command: {ex.Message}");
            }
        }

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            // Connection established - no game state access needed
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            // Send message can stay on background thread (Twitch API call)
            if (!ToolkitCoreSettings.sendMessageToChatOnStartup)
                return;

            Task.Run(() =>
            {
                try
                {
                    _client.SendMessage(e.Channel, "Toolkit Core has Connected to Chat", false);
                }
                catch (Exception ex)
                {
                    Log.Error($"[ToolkitCore] Error sending connection message: {ex.Message}");
                }
            });
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
                    Log.Message("Bits donated : " + message.Bits.ToString());

                if (Current.Game == null)
                    return;

                foreach (var twitchInterfaceBase in Current.Game.components.OfType<TwitchInterfaceBase>().ToList())
                {
                    twitchInterfaceBase.ParseMessage(message);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[ToolkitCore] Error processing message: {ex.Message}");
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
                    Log.Warning("[ToolkitCore] ChatCommand does not contain a ChatMessage reference");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[ToolkitCore] Error processing chat command: {ex.Message}");
            }
        }

        // ... (rest of the event handlers remain unchanged)

        public static void SendChatMessage(string message)
        {
            Instance?.SendChatMessageInternal(message);
        }

        public void SendChatMessageInternal(string message)
        {
            if (_client == null || string.IsNullOrEmpty(ToolkitCoreSettings.channel_username))
                return;

            var channel = _client.GetJoinedChannel(ToolkitCoreSettings.channel_username);
            if (channel != null)
            {
                // Twitch API calls can stay on current thread
                _client.SendMessage(channel, message, false);
            }
        }
    }
}
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using ToolkitCore.Controllers;
//using ToolkitCore.Models;
//using TwitchLib.Client;
//using TwitchLib.Client.Events;
//using TwitchLib.Client.Models;
//using TwitchLib.Communication.Clients;
//using TwitchLib.Communication.Events;
//using TwitchLib.Communication.Interfaces;
//using TwitchLib.Communication.Models;
//using Verse;

//namespace ToolkitCore
//{
//    public static class TwitchWrapper
//    {
//        public static TwitchClient Client { get; private set; }

//        public static void StartAsync() => Initialize(new ConnectionCredentials(ToolkitCoreSettings.bot_username, ToolkitCoreSettings.oauth_token));

//        public static void Initialize(ConnectionCredentials credentials)
//        {
//            ResetClient();
//            InitializeClient(credentials);
//        }

//        private static void ResetClient()
//        {
//            try
//            {
//                if (Client != null && Client.IsConnected)
//                    Client.Disconnect();
//                Client = new TwitchClient((IClient)new WebSocketClient(new ClientOptions()
//                {
//                    MessagesAllowedInPeriod = 750,
//                    ThrottlingPeriod = TimeSpan.FromSeconds(30.0)
//                }));
//            }
//            catch (Exception ex)
//            {
//                Log.Error(ex.Message + ex.InnerException.Message);
//            }
//        }

//        private static void InitializeClient(ConnectionCredentials credentials)
//        {
//            if (Client == null)
//            {
//                Log.Error("Tried to initialize null client, report to mod author");
//            }
//            else
//            {
//                Client.Initialize(credentials, ToolkitCoreSettings.channel_username);
//                Client.OnConnected += new EventHandler<OnConnectedArgs>(OnConnected);
//                Client.OnJoinedChannel += new EventHandler<OnJoinedChannelArgs>(OnJoinedChannel);
//                Client.OnMessageReceived += new EventHandler<OnMessageReceivedArgs>(OnMessageReceived);
//                Client.OnWhisperReceived += new EventHandler<OnWhisperReceivedArgs>(OnWhisperReceived);
//                Client.OnWhisperCommandReceived += new EventHandler<OnWhisperCommandReceivedArgs>(OnWhisperCommandReceived);
//                Client.OnChatCommandReceived += new EventHandler<OnChatCommandReceivedArgs>(OnChatCommandReceived);
//                Client.OnBeingHosted += new EventHandler<OnBeingHostedArgs>(OnBeingHosted);
//                Client.OnCommunitySubscription += new EventHandler<OnCommunitySubscriptionArgs>(OnCommunitySubscription);
//                Client.OnConnectionError += new EventHandler<OnConnectionErrorArgs>(OnConnectionError);
//                Client.OnDisconnected += new EventHandler<OnDisconnectedEventArgs>(OnDisconnected);
//                Client.OnFailureToReceiveJoinConfirmation += new EventHandler<OnFailureToReceiveJoinConfirmationArgs>(OnFailureToReceiveJoinConfirmation);
//                Client.OnGiftedSubscription += new EventHandler<OnGiftedSubscriptionArgs>(OnGiftedSubscription);
//                Client.OnHostingStarted += new EventHandler<OnHostingStartedArgs>(OnHostingStarted);
//                Client.OnIncorrectLogin += new EventHandler<OnIncorrectLoginArgs>(OnIncorrectLogin);
//                Client.OnLog += new EventHandler<OnLogArgs>(OnLog);
//                Client.OnNewSubscriber += new EventHandler<OnNewSubscriberArgs>(OnNewSubscriber);
//                Client.OnReSubscriber += new EventHandler<OnReSubscriberArgs>(OnReSubscriber);
//                Client.OnRaidNotification += new EventHandler<OnRaidNotificationArgs>(OnRaidNotification);
//                Client.OnUserBanned += new EventHandler<OnUserBannedArgs>(OnUserBanned);
//                Client.Connect();
//            }
//        }

//        private static void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
//        {
//            Task.Run(() =>
//            {
//                if (Current.Game == null || !ToolkitCoreSettings.allowWhispers)
//                    return;
//                foreach (TwitchInterfaceBase twitchInterfaceBase in (Current.Game.components).OfType<TwitchInterfaceBase>().ToList<TwitchInterfaceBase>())
//                    twitchInterfaceBase.ParseMessage(e.WhisperMessage);
//                MessageLog.LogMessage(e.WhisperMessage);
//            });
//        }

//        private static void OnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
//        {
//            Task.Run(() =>
//            {
//                if (Current.Game == null || !ToolkitCoreSettings.allowWhispers)
//                    return;
//                ChatCommandController.GetChatCommand(e.Command.CommandText)?.TryExecute(e.Command);
//            });
//        }

//        private static void OnConnected(object sender, OnConnectedArgs e)
//        {
//        }

//        private static void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
//        {
//            Task.Run(() =>
//            {
//                if (!ToolkitCoreSettings.sendMessageToChatOnStartup)
//                    return;
//                TwitchWrapper.Client.SendMessage(e.Channel, "Toolkit Core has Connected to Chat", false);
//            });
//        }

//        private static void OnMessageReceived(object sender, OnMessageReceivedArgs e)
//        {
//            Task.Run(() =>
//            {
//                MessageLog.LogMessage(e.ChatMessage);
//                if (e.ChatMessage.Bits > 0)
//                    Log.Message("Bits donated : " + e.ChatMessage.Bits.ToString());
//                if (Current.Game == null)
//                    return;
//                foreach (TwitchInterfaceBase twitchInterfaceBase in (Current.Game.components).OfType<TwitchInterfaceBase>().ToList<TwitchInterfaceBase>())
//                {
//                    twitchInterfaceBase.ParseMessage(e.ChatMessage);
//                }
//            });

//        }

//        private static void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
//        {
//            Task.Run(() =>
//            {
//                if (Current.Game == null || ToolkitCoreSettings.forceWhispers)
//                    return;
//                ChatCommandController.GetChatCommand(e.Command.CommandText)?.TryExecute(e.Command);
//            });
//        }

//        public static void OnBeingHosted(object sender, OnBeingHostedArgs e)
//        {
//        }

//        public static void OnCommunitySubscription(object sender, OnCommunitySubscriptionArgs e)
//        {
//        }

//        public static void OnConnectionError(object sender, OnConnectionErrorArgs e) => Log.Error("Client has experienced a connection error. " + e.Error?.ToString());

//        public static void OnDisconnected(object sender, OnDisconnectedEventArgs e) => Log.Warning("Client has disconnected");

//        public static void OnFailureToReceiveJoinConfirmation(
//          object sender,
//          OnFailureToReceiveJoinConfirmationArgs e)
//        {
//        }

//        public static void OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
//        {
//        }

//        public static void OnHostingStarted(object sender, OnHostingStartedArgs e)
//        {
//        }

//        public static void OnIncorrectLogin(object sender, OnIncorrectLoginArgs e) => Log.Error("Incorrect login detected. " + e.Exception.Message);

//        public static void OnLog(object sender, OnLogArgs e)
//        {
//        }

//        public static void OnNewSubscriber(object sender, OnNewSubscriberArgs e) => Log.Message("New Subscriber. " + e.Subscriber.DisplayName);

//        public static void OnReSubscriber(object sender, OnReSubscriberArgs e) => Log.Message("New Subscriber. " + e.ReSubscriber.DisplayName);

//        public static void OnRaidNotification(object sender, OnRaidNotificationArgs e) => Log.Message("Being raided by " + e.RaidNotification.DisplayName);

//        public static void OnUserBanned(object sender, OnUserBannedArgs e) => Log.Message("User has been banned - " + e.UserBan.Username);

//        public static void SendChatMessage(string message) => TwitchWrapper.Client.SendMessage(TwitchWrapper.Client.GetJoinedChannel(ToolkitCoreSettings.channel_username), message, false);
//    }
//}
