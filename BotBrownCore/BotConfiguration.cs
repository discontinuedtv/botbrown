namespace BotBrownCore
{
    public sealed class BotConfiguration
    {
        public BotConfiguration(string username, string accessToken, string channel)
        {
            Username = username;
            AccessToken = accessToken;
            Channel = channel;
        }

        public string Username { get; }

        public string AccessToken { get; }

        public string Channel { get; }
    }
}
