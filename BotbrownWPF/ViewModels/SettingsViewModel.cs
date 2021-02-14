using BotBrown.Configuration;
using BotbrownWPF.ViewModels.Configuration;
using System.ComponentModel;

namespace BotbrownWPF.ViewModels
{
    public class SettingsViewModel : ISettingsViewModel, IViewModel
    {
        public SettingsViewModel(IConfigurationManager configurationManager)
        {
            TwitchConfiguration = new TwitchConfigurationViewModel(configurationManager);
            GeneralConfiguration = new GeneralConfigurationViewModel(configurationManager);
            AudioConfiguration = new AudioConfigurationViewModel(configurationManager);
        }

        public TwitchConfigurationViewModel TwitchConfiguration { get; }

        public GeneralConfigurationViewModel GeneralConfiguration { get; }

        public AudioConfigurationViewModel AudioConfiguration { get; }
    }
}
