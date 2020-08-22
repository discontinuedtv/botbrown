namespace BotBrownCore.Configuration
{
    public class TwitchConfigurationFileFactory : IConfigurationFileFactory<TwitchConfiguration>
    {
        public TwitchConfiguration CreateDefaultConfiguration()
        {
            return new TwitchConfiguration
            {
                AccessToken = string.Empty,
                Channel = "Discontinuedman",
                Username = "Discontinuedman",
                TextToSpeechRewardId = "c15394f1-571b-4484-8d1c-b1b2384bae7a"
            };
        }
    }
}