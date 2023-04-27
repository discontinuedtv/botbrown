using BotBrown.Configuration;
using System.Collections.ObjectModel;
using System.Linq;

namespace BotbrownWPF.ViewModels
{
    public class SoundsPageViewModel : Notifier, ISoundsPageViewModel, IViewModel
    {
        private readonly IConfigurationManager configurationManager;
        private readonly SoundCommandConfiguration soundCommandConfiguration;

        public SoundsPageViewModel(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            soundCommandConfiguration = configurationManager.LoadConfiguration<SoundCommandConfiguration>();

            soundCommandConfiguration.CommandsDefinitions.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Sounds));
        }

        public ObservableCollection<SoundViewModel> Sounds
        {
            get
            {
                ObservableCollection<SoundViewModel> sounds = new ObservableCollection<SoundViewModel>();

                foreach (var definition in soundCommandConfiguration.CommandsDefinitions)
                {
                    sounds.Add(new SoundViewModel(definition, null));
                }

                return sounds;
            }
        }

        public void AddSound(SoundViewModel soundToAdd)
        {
            soundCommandConfiguration.AddDefinition(soundToAdd.Definition);
        }

        public string DestinationPathFor(string targetFileName)
        {
            return configurationManager.GenerateDestionationPathFor(targetFileName);
        }

        public bool HasExistingDefinitionForShortcut(string shortcut)
        {
            return soundCommandConfiguration.AllDefinitionKeys.Any(x => x == shortcut);
        }

        public void Save()
        {
            configurationManager.WriteConfiguration(soundCommandConfiguration);
            IsDirty = false;
        }
    }
}
