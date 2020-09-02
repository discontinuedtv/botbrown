namespace BotBrown.Workers.Twitch
{
    using BotBrown.Configuration;

    public interface ITwitchClientWrapper
    {
        void SendMessage(string channel, string message);

        void SendWhisper(string username, string message);

        void Stop();
        void ConnectToTwitch(TwitchConfiguration twitchConfiguration);
    }
}