namespace BotBrown.Workers.Twitch
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using System;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;
    using TwitchLib.Client.Models;
    using TwitchLib.Communication.Clients;
    using TwitchLib.Communication.Events;
    using TwitchLib.Communication.Models;

    public class TwitchClientWrapper : ITwitchClientWrapper
    {
        private readonly IUsernameResolver usernameResolver;
        private readonly IEventBus bus;
        private readonly ILogger logger;
        private TwitchClient client;

        public TwitchClientWrapper(IUsernameResolver usernameResolver, IEventBus bus, ILogger logger)
        {
            this.usernameResolver = usernameResolver;
            this.bus = bus;
            this.logger = logger;
        }

        public void ConnectToTwitch(TwitchConfiguration twitchConfiguration)
        {
            SetUpEvents();

            ConnectionCredentials credentials = new ConnectionCredentials(twitchConfiguration.Username, twitchConfiguration.AccessToken);
            ClientOptions clientOptions = CreateClientOptions();
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            InitializeTwitchClient(twitchConfiguration, credentials, customClient);
            RegisterClientCallbacks();

            client.Connect();
        }

        private void SetUpEvents()
        {
            bus.AddTopic<SubGiftEvent>();
            bus.AddTopic<NewSubscriberEvent>();
            bus.AddTopic<ResubscriberEvent>();
            bus.AddTopic<CommunitySubscriptionEvent>();
            bus.AddTopic<MessageReceivedEvent>();
            bus.AddTopic<TwitchChannelJoinedEvent>();
        }

        private void RegisterClientCallbacks()
        {
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnLeftChannel += Client_OnLeftChannel;

            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnCommunitySubscription += Client_OnCommunitySubscription;
            client.OnReSubscriber += Client_OnReSubscriber;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnGiftedSubscription += Client_OnGiftedSubscription;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
            client.OnWhisperCommandReceived += Client_OnWhisperCommandReceived;

            client.OnConnected += Client_OnConnected;
            client.OnDisconnected += Client_OnDisconnected;

            client.OnLog += Client_Log;
        }

        private void Client_OnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
        {
            var asd = e;
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            var asd = e;
        }

        private void Client_Log(object sender, OnLogArgs e)
        {
            //Console.WriteLine(e.Data);
        }

        private void InitializeTwitchClient(TwitchConfiguration twitchConfiguration, ConnectionCredentials credentials, WebSocketClient customClient)
        {
            client = new TwitchClient(customClient);
            client.Initialize(credentials, twitchConfiguration.Channel);
        }

        private static ClientOptions CreateClientOptions()
        {
            return new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            logger.Log($"Connected to {e.AutoJoinChannel}");
            bus.Publish(new ConnectedToTwitchEvent());
        }

        private void Client_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            logger.Log($"Disconnected");
            bus.Publish(new DisconnectedFromTwitchEvent());
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            TwitchChannelJoinedEvent channelJoined = new TwitchChannelJoinedEvent(e.Channel);
            bus.Publish(channelJoined);
        }

        private void Client_OnLeftChannel(object sender, OnLeftChannelArgs e)
        {
            logger.Log($"{e.BotUsername} left Channel {e.Channel}");
            bus.Publish(new TwitchChannelLeftEvent(e.Channel));
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e == null || sender == null)
            {
                return;
            }

            ChannelUser user = usernameResolver.ResolveUsername(new ChannelUser(e.ChatMessage.UserId, e.ChatMessage.DisplayName, e.ChatMessage.DisplayName));
            TwitchChatMessage chatMessage = new TwitchChatMessage(e.ChatMessage.Message, e.ChatMessage.IsBroadcaster, e.ChatMessage.IsModerator, e.ChatMessage.CustomRewardId, e.ChatMessage.Channel);
            MessageReceivedEvent message = new MessageReceivedEvent(user, chatMessage);

            bus.Publish(message);
        }

        private void Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            ChannelUser user = new ChannelUser(e.GiftedSubscription.MsgParamRecipientId, e.GiftedSubscription.MsgParamRecipientUserName, e.GiftedSubscription.MsgParamRecipientUserName);
            ChannelUser resolvedUser = usernameResolver.ResolveUsername(user);
            SubGiftEvent subgift = new SubGiftEvent(resolvedUser);

            bus.Publish(subgift);
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            ChannelUser user = new ChannelUser(e.Subscriber.UserId, e.Subscriber.DisplayName, e.Subscriber.DisplayName);
            ChannelUser resolvedUser = usernameResolver.ResolveUsername(user);
            NewSubscriberEvent newSubscriber = new NewSubscriberEvent(resolvedUser);

            bus.Publish(newSubscriber);
        }

        private void Client_OnReSubscriber(object sender, OnReSubscriberArgs resubscriberEventArguments)
        {
            ChannelUser user = new ChannelUser(resubscriberEventArguments.ReSubscriber.UserId, resubscriberEventArguments.ReSubscriber.DisplayName, resubscriberEventArguments.ReSubscriber.DisplayName);
            ChannelUser resolvedUser = usernameResolver.ResolveUsername(user);

            string months = !string.IsNullOrWhiteSpace(resubscriberEventArguments.ReSubscriber.MsgParamStreakMonths) ? resubscriberEventArguments.ReSubscriber.MsgParamStreakMonths : resubscriberEventArguments.ReSubscriber.MsgParamCumulativeMonths;

            if (!int.TryParse(months, out int numberOfMonthsSubscribed))
            {
                numberOfMonthsSubscribed = 1;
            }

            ResubscriberEvent resubscriber = new ResubscriberEvent(resolvedUser, numberOfMonthsSubscribed);
            bus.Publish(resubscriber);
        }

        private void Client_OnCommunitySubscription(object sender, OnCommunitySubscriptionArgs e)
        {
            ChannelUser user = new ChannelUser(e.GiftedSubscription.UserId, e.GiftedSubscription.DisplayName, e.GiftedSubscription.DisplayName);
            ChannelUser resolvedUser = usernameResolver.ResolveUsername(user);

            CommunitySubscriptionEvent communitySubscription = new CommunitySubscriptionEvent(resolvedUser, e.GiftedSubscription.MsgParamMassGiftCount);
            bus.Publish(communitySubscription);
        }

        public void Stop()
        {
            client?.Disconnect();
        }

        public void SendMessage(string channel, string replyMessage)
        {
            client.SendMessage(channel, replyMessage);
        }

        public void SendWhisper(string username, string message)
        {
            client.SendWhisper(username, message);
        }
    }
}