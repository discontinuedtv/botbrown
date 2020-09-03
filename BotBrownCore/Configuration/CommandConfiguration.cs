namespace BotBrown.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class CommandConfiguration : IConfiguration
    {
        public List<CommandDefinition> CommandsDefinitions { get; set; } = new List<CommandDefinition>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}