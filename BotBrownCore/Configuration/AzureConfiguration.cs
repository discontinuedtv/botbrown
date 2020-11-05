namespace BotBrown.Configuration
{
    using System.ComponentModel;

    [ConfigurationFile(ConfigurationFileConstants.Azure)]
    public class AzureConfiguration : IChangeableConfiguration
    {
        public string SubscriptionKey { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(SubscriptionKey);
        }
    }
}
