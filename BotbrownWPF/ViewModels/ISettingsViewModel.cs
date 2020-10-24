using BotbrownWPF.ViewModels.Configuration;

namespace BotbrownWPF.ViewModels
{
    public interface ISettingsViewModel : IViewModel
    {
        TwitchConfigurationViewModel TwitchConfiguration { get; }

        void Save();
    }
}