namespace BotBrownCore.Configuration
{
    internal class TwitchConfiguration : IConfiguration
    {
        public string Username { get; set; }

        public string AccessToken { get; set; }

        public string Channel { get; set; }

        public string TextToSpeechRewardId { get; set; }

        internal bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(AccessToken) &&
                !string.IsNullOrWhiteSpace(Channel);
        }
    }
}