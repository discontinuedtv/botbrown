namespace BotBrown.Configuration
{
    using BotBrown;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [ConfigurationFile(ConfigurationFileConstants.Commands)]
    public class CommandConfiguration : IConfiguration
    {
        private bool isInitialized;
        private Dictionary<string, CommandDefinition> definitions = new Dictionary<string, CommandDefinition>();
        private List<string> allDefinitionKeys = new List<string>();

        public List<CommandDefinition> CommandsDefinitions { get; set; } = new List<CommandDefinition>();

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
        }

        public bool IsValid()
        {
            return true;
        }
    }
}