namespace BotBrown.Workers.Twitch
{
    using BotBrown.Configuration;
    using BotBrown.Events;

    public interface ITwitchApiWrapper
    {
        void ConnectToTwitch(TwitchConfiguration twitchConfiguration);

        void Stop();
        void UpdateChannel(UpdateChannelEvent channelUpdate);
    }
}