namespace BotBrownCore
{
    using BotBrownCore.Configuration;
    using BotBrownCore.Events;

    public interface ITwitchClientWrapper
    {
        void SendMessage(string channel, string replyMessage);

        void SendWhisper(string username, string languages);

        void Stop();

        void Subscribe(Subscriber<SubGiftEvent> subscriber);

        void Subscribe(Subscriber<NewSubscriberEvent> subscriber);

        void Subscribe(Subscriber<ResubscriberEvent> subscriber);

        void Subscribe(Subscriber<CommunitySubscriptionEvent> subscriber);

        void Subscribe(Subscriber<MessageReceivedEvent> subscriber);

        void Subscribe(Subscriber<ChannelJoinedEvent> subscriber);

        void ConnectToTwitch(TwitchConfiguration twitchConfiguration);
    }
}