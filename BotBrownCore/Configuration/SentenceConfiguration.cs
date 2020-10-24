namespace BotBrown.Configuration
{
    using System.ComponentModel;

    [ConfigurationFile(ConfigurationFileConstants.Sentences)]
    public class SentenceConfiguration : IChangeableConfiguration
    {
        public string FollowerAlert { get; set; }

        public string SubscriberAlert { get; set; }

        public string GiftedSubscriberAlert { get; set; }

        public string ResubscriberAlert { get; set; }

        public string SubBombAlert { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Changed()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SentenceConfiguration)));
        }

        public bool IsValid()
        {
            return true;
        }
    }
}

