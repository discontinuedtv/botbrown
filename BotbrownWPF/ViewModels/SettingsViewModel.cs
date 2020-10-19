using BotBrown.Configuration;
using BotbrownWPF.ViewModels.Configuration;
using System.ComponentModel;

namespace BotbrownWPF.ViewModels
{
    public class SettingsViewModel : ISettingsViewModel, IViewModel, INotifyPropertyChanged
    {
        private bool isDirty;

        public SettingsViewModel(IConfigurationManager configurationManager)
        {
            TwitchConfiguration = new TwitchConfigurationViewModel(configurationManager);
            TwitchConfiguration.PropertyChanged += TwitchConfiguration_PropertyChanged;
        }

        private void TwitchConfiguration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IsDirty))
            {
                IsDirty = true;
            }
        }

        public void Save()
        {
            if (TwitchConfiguration.IsDirty)
            {
                TwitchConfiguration.Save();
            }
        }

        public TwitchConfigurationViewModel TwitchConfiguration { get; }

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }

            private set
            {
                if (value != isDirty)
                {
                    isDirty = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsDirty)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
