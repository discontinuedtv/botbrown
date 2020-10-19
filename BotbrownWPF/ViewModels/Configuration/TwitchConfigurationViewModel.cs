namespace BotbrownWPF.ViewModels.Configuration
{
    using BotBrown.Configuration;

    public class TwitchConfigurationViewModel : Notifier
    {
        private readonly IConfigurationManager configurationManager;
        private TwitchConfiguration twitchConfiguration;

        public TwitchConfigurationViewModel(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>(ConfigurationFileConstants.Twitch);
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

        public bool IsMissingAccessToken => string.IsNullOrWhiteSpace(twitchConfiguration.AccessToken);

        public void Save()
        {
            configurationManager.WriteConfiguration(twitchConfiguration, ConfigurationFileConstants.Twitch);
            IsDirty = false;
        }
    }
}
