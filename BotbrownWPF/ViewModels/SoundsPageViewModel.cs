using BotBrown.Configuration;
using System;
using System.Collections.ObjectModel;

namespace BotbrownWPF.ViewModels
{
    public class SoundsPageViewModel : ISoundsPageViewModel, IViewModel
    {
        private readonly IConfigurationManager configurationManager;
        private readonly SoundCommandConfiguration soundCommandConfiguration;

        public SoundsPageViewModel(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            soundCommandConfiguration = configurationManager.LoadConfiguration<SoundCommandConfiguration>();
        }

        public ObservableCollection<SoundViewModel> Sounds
        {
            get
            {
                ObservableCollection<SoundViewModel> sounds = new ObservableCollection<SoundViewModel>();

                foreach (var definition in soundCommandConfiguration.CommandsDefinitions)
                {
                    sounds.Add(new SoundViewModel
                    {
                        Name = definition.Name,
                        Volume = definition.Volume,
                        CooldownInSeconds = definition.CooldownInSeconds,
                        Filename = definition.Filename,
                        Shortcut = definition.Shortcut
                    });
                }

                return sounds;
            }
        }

        public void Save()
        {
            configurationManager.WriteConfiguration(soundCommandConfiguration);
        }
    }

    public class SoundViewModel : IViewModel
    {
        public string Name { get; set; }

        public float Volume { get; set; }

        public string Shortcut { get; set; }

        public string Filename { get; set; }

        public int CooldownInSeconds { get; set; }

        public string VolumeInPercent
        {
            get
            {
                return $"{Convert.ToInt32(Volume)}%";
            }
        }
    }
}
