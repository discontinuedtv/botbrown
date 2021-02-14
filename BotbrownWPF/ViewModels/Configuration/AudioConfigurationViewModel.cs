namespace BotbrownWPF.ViewModels.Configuration
{
    using BotBrown.Configuration;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class AudioConfigurationViewModel : Notifier
    {
        private readonly IConfigurationManager configurationManager;
        private readonly AudioConfiguration configuration;
        private readonly ObservableCollection<string> availableDeviceNames;

        public AudioConfigurationViewModel(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            configuration = configurationManager.LoadConfiguration<AudioConfiguration>();
            availableDeviceNames = new ObservableCollection<string>(configuration.AvailableDeviceNames.OrderBy(x => x));
        }

        public string SoundCommandDeviceName
        {
            get { return configuration.SoundCommandDeviceName; }
            set
            {
                if (value != configuration.SoundCommandDeviceName)
                {
                    configuration.SoundCommandDeviceName = value;
                    OnPropertyChanged(nameof(SoundCommandDeviceName));
                }
            }
        }

        public string TTSAudioDeviceName
        {
            get { return configuration.TTSAudioDeviceName; }
            set
            {
                if (value != configuration.TTSAudioDeviceName)
                {
                    configuration.TTSAudioDeviceName = value;
                    OnPropertyChanged(nameof(TTSAudioDeviceName));
                }
            }
        }

        public ObservableCollection<string> AvailableAudioDevices
        {
            get { return availableDeviceNames; }
        }

        public void Save()
        {
            configurationManager.WriteConfiguration(configuration);
            IsDirty = false;
        }
    }
}
