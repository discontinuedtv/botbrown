namespace BotBrown.Configuration
{
    public class SentenceConfiguration : IConfiguration
    {
        public string FollowerAlert { get; set; }

        public string SubscriberAlert { get; set; }

        public string GiftedSubscriberAlert { get; set; }

        public string ResubscriberAlert { get; set; }

        public string SubBombAlert { get; set; }
    }
}

