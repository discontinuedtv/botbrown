using BotBrown.Configuration;

namespace BotbrownWPF.ViewModels.Configuration
{
    public class GeneralConfigurationViewModel : Notifier
    {
        private readonly IConfigurationManager configurationManager;
        private readonly GeneralConfiguration configuration;

        public GeneralConfigurationViewModel(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            configuration = configurationManager.LoadConfiguration<GeneralConfiguration>();
        }

        public bool TtsActive
        {
            get { return configuration.ActivateTextToSpeech; }
            set
            {
                if (value != configuration.ActivateTextToSpeech)
                {
                    configuration.ActivateTextToSpeech = value;
                    OnPropertyChanged(nameof(TtsActive));
                }
            }
        }

        public string ByePhrase
        {
            get { return configuration.ByePhrase; }
            set
            {
                if (value != configuration.ByePhrase)
                {
                    configuration.ByePhrase = value;
                    OnPropertyChanged(nameof(ByePhrase));
                }
            }
        }

        public string BotChannelGreeting
        {
            get { return configuration.BotChannelGreeting; }
            set
            {
                if (value != configuration.BotChannelGreeting)
                {
                    configuration.BotChannelGreeting = value;
                    OnPropertyChanged(nameof(BotChannelGreeting));
                }
            }
        }

        public void Save()
        {
            configurationManager.WriteConfiguration(configuration);
            IsDirty = false;
        }
    }
}
