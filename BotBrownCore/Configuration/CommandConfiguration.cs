namespace BotBrownCore.Configuration
{
    using System.Collections.Generic;

    internal class CommandConfiguration : IConfiguration
    {
        public List<CommandDefinition> CommandsDefinitions { get; set; } = new List<CommandDefinition>();
    }
}