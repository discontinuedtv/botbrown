namespace BotBrownCore.Workers.Twitch
{
    using BotBrownCore.Configuration;

    public interface ITwitchApiWrapper
    {
        void ConnectToTwitch(TwitchConfiguration twitchConfiguration);

        void Stop();
    }
}