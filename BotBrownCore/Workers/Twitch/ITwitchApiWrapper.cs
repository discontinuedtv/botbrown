namespace BotBrown.Workers.Twitch
{
    using System;
    using System.Threading.Tasks;
    using BotBrown.Configuration;
    using BotBrown.Events;

    public interface ITwitchApiWrapper
    {
        void ConnectToTwitch(TwitchConfiguration twitchConfiguration);

        void Stop();
        void UpdateChannel(UpdateChannelEvent channelUpdate);

        Task<string> GetCurrentGame();

        Task<string> GetUserIdByUsername(string username);

        Task<DateTime?> GetFollowSince(string userId)
    }
}