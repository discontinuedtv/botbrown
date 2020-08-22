namespace BotBrownCore
{
    using BotBrownCore.Configuration;
    using BotBrownCore.Events;

    public interface ITwitchApiWrapper
    {
        void ConnectToTwitch(TwitchConfiguration twitchConfiguration);

        void Stop();

        void Subscribe(Subscriber<NewFollowerEvent> subscriber);
    }
}