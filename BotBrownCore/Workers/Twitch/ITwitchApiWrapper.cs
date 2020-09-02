namespace BotBrown.Workers.Twitch
{
    using BotBrown.Configuration;

    public interface ITwitchApiWrapper
    {
        void ConnectToTwitch(TwitchConfiguration twitchConfiguration);

        void Stop();
    }
}