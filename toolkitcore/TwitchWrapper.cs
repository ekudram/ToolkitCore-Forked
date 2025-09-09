/*
 * File: TwitchWrapper.cs
 * Project: ToolkitCore
 * 
 * Updated: October 26, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1.  Converted from static class to instance class to properly access instance-based settings.
 * 2.  Added reference to ToolkitCore mod instance to access settings.
 * 3.  Updated all method signatures to be instance methods instead of static.
 * 4.  Modified all settings references to use the instance-based approach.
 * 5.  Added error handling for null settings references.
 * 6.  Changed initial connection error from Error to Warning as requested.
 * 
 * Why These Changes Were Made:
 * The TwitchWrapper class was previously accessing settings through static fields,
 * which conflicted with our changes to make settings instance-based for better mod reload safety.
 * Converting to an instance class allows proper access to the settings through the mod instance.
 * The change from Error to Warning for initial connection errors provides a better user experience
 * for first-time users who haven't configured their Twitch credentials yet.
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
        private ToolkitCoreSettings Settings => _mod.Settings;

        public TwitchClient Client { get; private set; }

        public TwitchWrapper(ToolkitCore mod)
        {
            _mod = mod;
        }

        public void StartAsync()
        {
            if (Settings == null)
            {
                Log.Warning("Cannot start Twitch connection: Settings are null");
                return;
            }

            Initialize(new ConnectionCredentials(Settings.bot_username, Settings.oauth_token));
        }

        public void Initialize(ConnectionCredentials credentials)
        {
            if (Settings == null)
            {
                Log.Warning("Cannot initialize Twitch connection: Settings are null");
                return;
            }

            ResetClient();
            InitializeClient(credentials);
        }

        private void ResetClient()
        {
            try
            {
                if (Client != null && Client.IsConnected)
                    Client.Disconnect();

                Client = new TwitchClient(new WebSocketClient(new ClientOptions()
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
            if (Client == null)
            {
                Log.Error("Tried to initialize null client, report to mod author");
                return;
            }

            if (Settings == null)
            {
                Log.Error("Cannot initialize Twitch client: Settings are null");
                return;
            }

            Client.Initialize(credentials, Settings.channel_username);
            Client.OnConnected += OnConnected;
            Client.OnJoinedChannel += OnJoinedChannel;
            Client.OnMessageReceived += OnMessageReceived;
            Client.OnWhisperReceived += OnWhisperReceived;
            Client.OnWhisperCommandReceived += OnWhisperCommandReceived;
            Client.OnChatCommandReceived += OnChatCommandReceived;
            Client.OnBeingHosted += OnBeingHosted;
            Client.OnCommunitySubscription += OnCommunitySubscription;
            Client.OnConnectionError += OnConnectionError;
            Client.OnDisconnected += OnDisconnected;
            Client.OnFailureToReceiveJoinConfirmation += OnFailureToReceiveJoinConfirmation;
            Client.OnGiftedSubscription += OnGiftedSubscription;
            Client.OnHostingStarted += OnHostingStarted;
            Client.OnIncorrectLogin += OnIncorrectLogin;
            Client.OnLog += OnLog;
            Client.OnNewSubscriber += OnNewSubscriber;
            Client.OnReSubscriber += OnReSubscriber;
            Client.OnRaidNotification += OnRaidNotification;
            Client.OnUserBanned += OnUserBanned;
            Client.Connect();
        }

        private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            Task.Run(() =>
            {
                if (Current.Game == null || (Settings != null && !Settings.allowWhispers))
                    return;

                foreach (var twitchInterfaceBase in Current.Game.components.OfType<TwitchInterfaceBase>().ToList())
                    twitchInterfaceBase.ParseMessage(e.WhisperMessage);

                MessageLog.LogMessage(e.WhisperMessage);
            });
        }

        private void OnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
        {
            Task.Run(() =>
            {
                if (Current.Game == null || (Settings != null && !Settings.allowWhispers))
                    return;

                ChatCommandController.GetChatCommand(e.Command.CommandText)?.TryExecute(e.Command);
            });
        }

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            // Connection established
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Task.Run(() =>
            {
                if (Settings != null && !Settings.sendMessageToChatOnStartup)
                    return;

                Client.SendMessage(e.Channel, "Toolkit Core has Connected to Chat", false);
            });
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Task.Run(() =>
            {
                MessageLog.LogMessage(e.ChatMessage);

                if (e.ChatMessage.Bits > 0)
                    Log.Message("Bits donated : " + e.ChatMessage.Bits.ToString());

                if (Current.Game == null)
                    return;

                foreach (var twitchInterfaceBase in Current.Game.components.OfType<TwitchInterfaceBase>().ToList())
                {
                    twitchInterfaceBase.ParseMessage(e.ChatMessage);
                }
            });
        }

        private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            Task.Run(() =>
            {
                if (Current.Game == null || (Settings != null && Settings.forceWhispers))
                    return;

                ChatCommandController.GetChatCommand(e.Command.CommandText)?.TryExecute(e.Command);
            });
        }

        public void OnBeingHosted(object sender, OnBeingHostedArgs e)
        {
            // Being hosted event
        }

        public void OnCommunitySubscription(object sender, OnCommunitySubscriptionArgs e)
        {
            // Community subscription event
        }

        public void OnConnectionError(object sender, OnConnectionErrorArgs e) =>
            Log.Warning("Client has experienced a connection error. " + e.Error?.ToString());

        public void OnDisconnected(object sender, OnDisconnectedEventArgs e) =>
            Log.Warning("Client has disconnected");

        public void OnFailureToReceiveJoinConfirmation(object sender, OnFailureToReceiveJoinConfirmationArgs e)
        {
            // Failed to receive join confirmation
        }

        public void OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            // Gifted subscription event
        }

        public void OnHostingStarted(object sender, OnHostingStartedArgs e)
        {
            // Hosting started event
        }

        public void OnIncorrectLogin(object sender, OnIncorrectLoginArgs e) =>
            Log.Error("Incorrect login detected. " + e.Exception.Message);

        public void OnLog(object sender, OnLogArgs e)
        {
            // Log event
        }

        public void OnNewSubscriber(object sender, OnNewSubscriberArgs e) =>
            Log.Message("New Subscriber. " + e.Subscriber.DisplayName);

        public void OnReSubscriber(object sender, OnReSubscriberArgs e) =>
            Log.Message("New Subscriber. " + e.ReSubscriber.DisplayName);

        public void OnRaidNotification(object sender, OnRaidNotificationArgs e) =>
            Log.Message("Being raided by " + e.RaidNotification.DisplayName);

        public void OnUserBanned(object sender, OnUserBannedArgs e) =>
            Log.Message("User has been banned - " + e.UserBan.Username);

        public void SendChatMessage(string message)
        {
            if (Settings == null || Client == null)
                return;

            var channel = Client.GetJoinedChannel(Settings.channel_username);
            if (channel != null)
            {
                Client.SendMessage(channel, message, false);
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
