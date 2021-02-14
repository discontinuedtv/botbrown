using BotbrownWPF.ViewModels.Configuration;

namespace BotbrownWPF.ViewModels
{
    public interface ISettingsViewModel : IViewModel
    {
        AudioConfigurationViewModel AudioConfiguration { get; }

        TwitchConfigurationViewModel TwitchConfiguration { get; }

        GeneralConfigurationViewModel GeneralConfiguration { get; }
    }
}