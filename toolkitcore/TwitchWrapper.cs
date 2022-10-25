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
    public static class TwitchWrapper
    {
        public static TwitchClient Client { get; private set; }

        public static void StartAsync() => Initialize(new ConnectionCredentials(ToolkitCoreSettings.bot_username, ToolkitCoreSettings.oauth_token));

        public static void Initialize(ConnectionCredentials credentials)
        {
            ResetClient();
            InitializeClient(credentials);
        }

        private static void ResetClient()
        {
            try
            {
                if (Client != null && Client.IsConnected)
                    Client.Disconnect();
                Client = new TwitchClient((IClient)new WebSocketClient(new ClientOptions()
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30.0)
                }));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + ex.InnerException.Message);
            }
        }

        private static void InitializeClient(ConnectionCredentials credentials)
        {
            if (Client == null)
            {
                Log.Error("Tried to initialize null client, report to mod author");
            }
            else
            {
                Client.Initialize(credentials, ToolkitCoreSettings.channel_username);
                Client.OnConnected += new EventHandler<OnConnectedArgs>(OnConnected);
                Client.OnJoinedChannel += new EventHandler<OnJoinedChannelArgs>(OnJoinedChannel);
                Client.OnMessageReceived += new EventHandler<OnMessageReceivedArgs>(OnMessageReceived);
                Client.OnWhisperReceived += new EventHandler<OnWhisperReceivedArgs>(OnWhisperReceived);
                Client.OnWhisperCommandReceived += new EventHandler<OnWhisperCommandReceivedArgs>(OnWhisperCommandReceived);
                Client.OnChatCommandReceived += new EventHandler<OnChatCommandReceivedArgs>(OnChatCommandReceived);
                Client.OnBeingHosted += new EventHandler<OnBeingHostedArgs>(OnBeingHosted);
                Client.OnCommunitySubscription += new EventHandler<OnCommunitySubscriptionArgs>(OnCommunitySubscription);
                Client.OnConnectionError += new EventHandler<OnConnectionErrorArgs>(OnConnectionError);
                Client.OnDisconnected += new EventHandler<OnDisconnectedEventArgs>(OnDisconnected);
                Client.OnFailureToReceiveJoinConfirmation += new EventHandler<OnFailureToReceiveJoinConfirmationArgs>(OnFailureToReceiveJoinConfirmation);
                Client.OnGiftedSubscription += new EventHandler<OnGiftedSubscriptionArgs>(OnGiftedSubscription);
                Client.OnHostingStarted += new EventHandler<OnHostingStartedArgs>(OnHostingStarted);
                Client.OnIncorrectLogin += new EventHandler<OnIncorrectLoginArgs>(OnIncorrectLogin);
                Client.OnLog += new EventHandler<OnLogArgs>(OnLog);
                Client.OnNewSubscriber += new EventHandler<OnNewSubscriberArgs>(OnNewSubscriber);
                Client.OnReSubscriber += new EventHandler<OnReSubscriberArgs>(OnReSubscriber);
                Client.OnRaidNotification += new EventHandler<OnRaidNotificationArgs>(OnRaidNotification);
                Client.OnUserBanned += new EventHandler<OnUserBannedArgs>(OnUserBanned);
                Client.Connect();
            }
        }

        private static void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            Task.Run(() =>
            {
                if (Current.Game == null || !ToolkitCoreSettings.allowWhispers)
                    return;
                foreach (TwitchInterfaceBase twitchInterfaceBase in (Current.Game.components).OfType<TwitchInterfaceBase>().ToList<TwitchInterfaceBase>())
                    twitchInterfaceBase.ParseMessage(e.WhisperMessage);
                MessageLog.LogMessage(e.WhisperMessage);
            });
        }

        private static void OnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
        {
            Task.Run(() =>
            {
                if (Current.Game == null || !ToolkitCoreSettings.allowWhispers)
                    return;
                ChatCommandController.GetChatCommand(e.Command.CommandText)?.TryExecute(e.Command);
            });
        }

        private static void OnConnected(object sender, OnConnectedArgs e)
        {
        }

        private static void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Task.Run(() =>
            {
                if (!ToolkitCoreSettings.sendMessageToChatOnStartup)
                    return;
                TwitchWrapper.Client.SendMessage(e.Channel, "Toolkit Core has Connected to Chat", false);
            });
        }

        private static void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Task.Run(() =>
            {
                MessageLog.LogMessage(e.ChatMessage);
                if (e.ChatMessage.Bits > 0)
                    Log.Message("Bits donated : " + e.ChatMessage.Bits.ToString());
                if (Current.Game == null)
                    return;
                foreach (TwitchInterfaceBase twitchInterfaceBase in (Current.Game.components).OfType<TwitchInterfaceBase>().ToList<TwitchInterfaceBase>())
                {
                    twitchInterfaceBase.ParseMessage(e.ChatMessage);
                }
            });

        }

        private static void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            Task.Run(() =>
            {
                if (Current.Game == null || ToolkitCoreSettings.forceWhispers)
                    return;
                ChatCommandController.GetChatCommand(e.Command.CommandText)?.TryExecute(e.Command);
            });
        }

        public static void OnBeingHosted(object sender, OnBeingHostedArgs e)
        {
        }

        public static void OnCommunitySubscription(object sender, OnCommunitySubscriptionArgs e)
        {
        }

        public static void OnConnectionError(object sender, OnConnectionErrorArgs e) => Log.Error("Client has experienced a connection error. " + e.Error?.ToString());

        public static void OnDisconnected(object sender, OnDisconnectedEventArgs e) => Log.Warning("Client has disconnected");

        public static void OnFailureToReceiveJoinConfirmation(
          object sender,
          OnFailureToReceiveJoinConfirmationArgs e)
        {
        }

        public static void OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
        }

        public static void OnHostingStarted(object sender, OnHostingStartedArgs e)
        {
        }

        public static void OnIncorrectLogin(object sender, OnIncorrectLoginArgs e) => Log.Error("Incorrect login detected. " + e.Exception.Message);

        public static void OnLog(object sender, OnLogArgs e)
        {
        }

        public static void OnNewSubscriber(object sender, OnNewSubscriberArgs e) => Log.Message("New Subscriber. " + e.Subscriber.DisplayName);

        public static void OnReSubscriber(object sender, OnReSubscriberArgs e) => Log.Message("New Subscriber. " + e.ReSubscriber.DisplayName);

        public static void OnRaidNotification(object sender, OnRaidNotificationArgs e) => Log.Message("Being raided by " + e.RaidNotification.DisplayName);

        public static void OnUserBanned(object sender, OnUserBannedArgs e) => Log.Message("User has been banned - " + e.UserBan.Username);

        public static void SendChatMessage(string message) => TwitchWrapper.Client.SendMessage(TwitchWrapper.Client.GetJoinedChannel(ToolkitCoreSettings.channel_username), message, false);
    }
}
