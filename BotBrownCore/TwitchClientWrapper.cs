namespace BotBrownCore
{
    using BotBrownCore.Configuration;
    using BotBrownCore.Events;
    using System;
    using System.Collections.Generic;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;
    using TwitchLib.Client.Models;
    using TwitchLib.Communication.Clients;
    using TwitchLib.Communication.Events;
    using TwitchLib.Communication.Models;

    public class TwitchClientWrapper : ITwitchClientWrapper
    {
        private TwitchClient client;

        private List<Subscriber<SubGiftEvent>> subGiftEventSubscriber = new List<Subscriber<SubGiftEvent>>();
        private List<Subscriber<NewSubscriberEvent>> newSubscriberEventSubscriber = new List<Subscriber<NewSubscriberEvent>>();
        private List<Subscriber<ResubscriberEvent>> resubscriberEventSubscriber = new List<Subscriber<ResubscriberEvent>>();
        private List<Subscriber<CommunitySubscriptionEvent>> communitySubscriptionEventSubscriber = new List<Subscriber<CommunitySubscriptionEvent>>();
        private List<Subscriber<MessageReceivedEvent>> messageReceivedEventSubscriber = new List<Subscriber<MessageReceivedEvent>>();
        private List<Subscriber<ChannelJoinedEvent>> channelJoinedEventSubscriber = new List<Subscriber<ChannelJoinedEvent>>();
        
        public void Subscribe(Subscriber<SubGiftEvent> subscriber)
        {
            subGiftEventSubscriber.Add(subscriber);
        }

        public void Subscribe(Subscriber<NewSubscriberEvent> subscriber)
        {
            newSubscriberEventSubscriber.Add(subscriber);
        }

        public void Subscribe(Subscriber<ResubscriberEvent> subscriber)
        {
            resubscriberEventSubscriber.Add(subscriber);
        }

        public void Subscribe(Subscriber<CommunitySubscriptionEvent> subscriber)
        {
            communitySubscriptionEventSubscriber.Add(subscriber);
        }

        public void Subscribe(Subscriber<MessageReceivedEvent> subscriber)
        {
            messageReceivedEventSubscriber.Add(subscriber);
        }

        public void Subscribe(Subscriber<ChannelJoinedEvent> subscriber)
        {
            channelJoinedEventSubscriber.Add(subscriber);
        }

        public void ConnectToTwitch(TwitchConfiguration twitchConfiguration)
        {
            ConnectionCredentials credentials = new ConnectionCredentials(twitchConfiguration.Username, twitchConfiguration.AccessToken);
            ClientOptions clientOptions = CreateClientOptions();
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            InitializeTwitchClient(twitchConfiguration, credentials, customClient);
            RegisterClientCallbacks();

            client.Connect();
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

            client.OnConnected += Client_OnConnected;
            client.OnDisconnected += Client_OnDisconnected;
            
            client.OnLog += Client_Log;
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
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            Console.WriteLine($"Disconnected");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            ChannelJoinedEvent channelJoined = new ChannelJoinedEvent(e.Channel);

            foreach (Subscriber<ChannelJoinedEvent> subscriber in channelJoinedEventSubscriber)
            {
                subscriber.Notify(channelJoined);
            }
        }

        private void Client_OnLeftChannel(object sender, OnLeftChannelArgs e)
        {
            var asd = e;

        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e == null || sender == null)
            {
                return;
            }

            ChannelUser user = new ChannelUser(e.ChatMessage.UserId, e.ChatMessage.DisplayName, e.ChatMessage.DisplayName);
            ChatMessage chatMessage = new ChatMessage(this, e.ChatMessage.Message, e.ChatMessage.IsBroadcaster, e.ChatMessage.IsModerator, e.ChatMessage.CustomRewardId, e.ChatMessage.Channel, e.ChatMessage.Username);
            MessageReceivedEvent message = new MessageReceivedEvent(user, chatMessage);

            foreach (Subscriber<MessageReceivedEvent> subscriber in messageReceivedEventSubscriber)
            {
                subscriber.Notify(message);
            }
        }

        private void Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            ChannelUser user = new ChannelUser(e.GiftedSubscription.MsgParamRecipientId, e.GiftedSubscription.MsgParamRecipientUserName, e.GiftedSubscription.MsgParamRecipientUserName);
            SubGiftEvent subgift = new SubGiftEvent(user);

            foreach (Subscriber<SubGiftEvent> subscriber in subGiftEventSubscriber)
            {
                subscriber.Notify(subgift);
            }
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            ChannelUser user = new ChannelUser(e.Subscriber.UserId, e.Subscriber.DisplayName, e.Subscriber.DisplayName);
            NewSubscriberEvent newSubscriber = new NewSubscriberEvent(user);

            foreach (Subscriber<NewSubscriberEvent> subscriber in newSubscriberEventSubscriber)
            {
                subscriber.Notify(newSubscriber);
            }           
        }

        private void Client_OnReSubscriber(object sender, OnReSubscriberArgs resubscriberEventArguments)
        {
            ChannelUser user = new ChannelUser(resubscriberEventArguments.ReSubscriber.UserId, resubscriberEventArguments.ReSubscriber.DisplayName, resubscriberEventArguments.ReSubscriber.DisplayName);

            string months = !string.IsNullOrWhiteSpace(resubscriberEventArguments.ReSubscriber.MsgParamStreakMonths) ? resubscriberEventArguments.ReSubscriber.MsgParamStreakMonths : resubscriberEventArguments.ReSubscriber.MsgParamCumulativeMonths;

            if (!int.TryParse(months, out int numberOfMonthsSubscribed))
            {
                numberOfMonthsSubscribed = 1;
            }

            ResubscriberEvent resubscriber = new ResubscriberEvent(user, numberOfMonthsSubscribed);

            foreach (Subscriber<ResubscriberEvent> subscriber in resubscriberEventSubscriber)
            {
                subscriber.Notify(resubscriber);
            }
        }

        private void Client_OnCommunitySubscription(object sender, OnCommunitySubscriptionArgs e)
        {
            ChannelUser user = new ChannelUser(e.GiftedSubscription.UserId, e.GiftedSubscription.DisplayName, e.GiftedSubscription.DisplayName);
            CommunitySubscriptionEvent communitySubscription = new CommunitySubscriptionEvent(user, e.GiftedSubscription.MsgParamMassGiftCount);

            foreach (Subscriber<CommunitySubscriptionEvent> subscriber in communitySubscriptionEventSubscriber)
            {
                subscriber.Notify(communitySubscription);
            }
        }

        public void Stop()
        {
            client.Disconnect();
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