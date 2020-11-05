namespace BotBrown.Configuration
{
    [ConfigurationFile(ConfigurationFileConstants.Azure)]
    public class AzureConfiguration : IConfiguration
    {
        public string SubscriptionKey { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(SubscriptionKey);
        }
    }
}
