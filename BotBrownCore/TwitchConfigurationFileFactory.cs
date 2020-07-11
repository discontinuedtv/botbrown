namespace BotBrownCore
{
    internal class TwitchConfigurationFileFactory : IConfigurationFileFactory<TwitchConfiguration>
    {
        public TwitchConfiguration CreateDefaultConfiguration()
        {
            return new TwitchConfiguration
            {
                AccessToken = string.Empty,
                Channel = "Discontinuedman",
                Username = "Discontinuedman"
            };
        }
    }
}