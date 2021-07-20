namespace BotBrown.Workers.Twitch
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using System.Threading.Tasks;

    public interface ITwitchApiWrapper
    {
        void ConnectToTwitch(TwitchConfiguration twitchConfiguration);

        void Stop();

        Task UpdateChannel(UpdateChannelEvent channelUpdate);

        Task<string> GetCurrentGame();

        Task<string> GetUserIdByUsername(string username);
    }
}
