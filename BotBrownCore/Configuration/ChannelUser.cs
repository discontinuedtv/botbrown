namespace BotBrown.Configuration
{
    public sealed class ChannelUser
    {
        public ChannelUser(string userId, string realUsername, string username)
        {
            UserId = userId;
            RealUsername = realUsername;
            Username = username;
        }

        public string UserId { get; }

        public string RealUsername { get; }

        public string Username { get; }

        internal ChannelUser WithResolvedUsername(string resolvedUsername)
        {
            return new ChannelUser(UserId, RealUsername, resolvedUsername);
        }
    }
}