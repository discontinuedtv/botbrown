namespace BotbrownWPF.ViewModels.Configuration
{
    using BotBrown.Configuration;

    public class TwitchConfigurationViewModel : Notifier
    {
        private readonly IConfigurationManager configurationManager;
        private readonly TwitchConfiguration twitchConfiguration;

        public TwitchConfigurationViewModel(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>();
        }

        public string Channel
        {
            get { return twitchConfiguration.Channel; }
            set
            {
                if (value != twitchConfiguration.Channel)
                {
                    twitchConfiguration.Channel = value;
                    OnPropertyChanged(nameof(Channel));
                }
            }
        }

        public string AccessToken
        {
            get { return twitchConfiguration.AccessToken; }
            set
            {
                if (value != twitchConfiguration.AccessToken)
                {
                    twitchConfiguration.AccessToken = value;
                    OnPropertyChanged(nameof(AccessToken));
                }
            }
        }

        public string BroadcasterUserId 
        {
            get { return twitchConfiguration.BroadcasterUserId; }
            set
            {
                if (value != twitchConfiguration.AccessToken)
                {
                    twitchConfiguration.BroadcasterUserId = value;
                    OnPropertyChanged(nameof(BroadcasterUserId));
                }
            }
        }

        public bool IsMissingAccessToken => string.IsNullOrWhiteSpace(twitchConfiguration.AccessToken)
            && !string.IsNullOrWhiteSpace(twitchConfiguration.Channel);

        public void Save()
        {
            configurationManager.WriteConfiguration(twitchConfiguration);
            IsDirty = false;
        }
    }
}
