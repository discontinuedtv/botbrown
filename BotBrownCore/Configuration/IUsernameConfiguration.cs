namespace BotBrown.Configuration
{
    public interface IUsernameConfiguration : IChangeableConfiguration
    {
        void AddUsername(ChannelUser user);

        bool TryGetValue(string userId, out string username);

        bool FindUserByRealUsername(string realUsername, out ChannelUser user);
    }
}