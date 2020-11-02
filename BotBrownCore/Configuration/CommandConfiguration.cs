namespace BotBrown.Configuration
{
    using BotBrown;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;

    [ConfigurationFile(ConfigurationFileConstants.Commands)]
    public class SoundCommandConfiguration : IChangeableConfiguration
    {
        private bool isInitialized;
        private readonly Dictionary<string, CommandDefinition> definitions = new Dictionary<string, CommandDefinition>();
        private readonly List<string> allDefinitionKeys = new List<string>();
        public ObservableCollection<CommandDefinition> CommandsDefinitions { get; set; } = new ObservableCollection<CommandDefinition>();
        public event PropertyChangedEventHandler PropertyChanged;

        public SoundCommandConfiguration()
        {
            CommandsDefinitions.CollectionChanged += CommandsDefinitions_CollectionChanged;
        }

        private void CommandsDefinitions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CommandsDefinitions)));
        }
        
        public List<string> AllDefinitionKeys
        {
            get
            {
                if (!isInitialized)
                {
                    Initialize();
                }

                return allDefinitionKeys.ToList();
            }
        }

        public void AddDefinition(CommandDefinition definition)
        {
            CommandsDefinitions.Add(definition);

            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(CommandsDefinitions)));
            }
        }

        public bool TryGetDefinition(string shortcut, out CommandDefinition definition)
        {
            if (!isInitialized)
            {
                Initialize();
            }

            return definitions.TryGetValue(shortcut, out definition);
        }

        private void Initialize()
        {
            foreach (CommandDefinition commandDefinition in CommandsDefinitions)
            {
                allDefinitionKeys.Add(commandDefinition.Shortcut);
                definitions.Add(commandDefinition.Shortcut, commandDefinition);
            }

            isInitialized = true;
        }

        public bool IsValid()
        {
            return true;
        }
    }
}