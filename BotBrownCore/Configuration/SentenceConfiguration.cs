namespace BotBrown.Configuration
{
    [ConfigurationFile(ConfigurationFileConstants.Sentences)]
    public class SentenceConfiguration : IConfiguration
    {
        public string FollowerAlert { get; set; }

        public string SubscriberAlert { get; set; }

        public string GiftedSubscriberAlert { get; set; }

        public string ResubscriberAlert { get; set; }

        public string SubBombAlert { get; set; }

        public bool IsValid()
        {
            return true;
        }
    }
}

