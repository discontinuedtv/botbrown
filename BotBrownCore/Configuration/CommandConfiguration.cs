namespace BotBrown.Configuration
{
    using BotBrown;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Collections.ObjectModel;

    [ConfigurationFile(ConfigurationFileConstants.Commands)]
    public class CommandConfiguration : IChangeableConfiguration
    {
        private bool isInitialized;
        private Dictionary<string, CommandDefinition> definitions = new Dictionary<string, CommandDefinition>();
        private List<string> allDefinitionKeys = new List<string>();

        public ObservableCollection<CommandDefinition> CommandsDefinitions { get; set; } = new ObservableCollection<CommandDefinition>();

        public CommandConfiguration()
        {
            CommandsDefinitions.CollectionChanged += CommandsDefinitions_CollectionChanged;
        }

        private void CommandsDefinitions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CommandsDefinitions)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

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