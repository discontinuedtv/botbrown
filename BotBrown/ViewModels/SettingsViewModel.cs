using Avalonia;
using BotBrown.Configuration;

namespace BotBrown.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IConfigurationManager configurationManager;
        private readonly TwitchConfiguration twitchConfiguration;

        public SettingsViewModel()
        {
            configurationManager = AvaloniaLocator.Current.GetService<IConfigurationManager>();

            twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>(ConfigurationFileConstants.Twitch);

        }

        public string AccessToken => twitchConfiguration.AccessToken;
        //public string AccessToken => "asd"; //twitchConfiguration.AccessToken;
        public string Channel => twitchConfiguration.Channel;
        //public string Channel => "qwe"; // twitchConfiguration.Channel;
    }
}
