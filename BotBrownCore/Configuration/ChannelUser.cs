namespace BotBrownCore.Configuration
{
    public sealed class ChannelUser
    {
        public ChannelUser(string userId, string realUsername, string username)
        {
            UserId = userId;
            RealUsername = realUsername;
            Username = username;
        }

        public string UserId { get; set; }

        public string RealUsername { get; set; }

        public string Username { get; set; }
    }
}