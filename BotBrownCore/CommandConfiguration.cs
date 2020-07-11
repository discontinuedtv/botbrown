using System;
using System.Collections.Generic;

namespace BotBrownCore
{
    internal class CommandConfiguration : IConfiguration
    {
        private List<CommandDefinition> commands = new List<CommandDefinition>();

        public List<CommandDefinition> CommandsDefinitions 
        { 
            get { return commands; } 
            set { commands = value; } 
        }
    }

    internal class CommandDefinition
    {
        public string Shortcut { get; set; }

        public string Name { get; set; }

        public string Filename { get; set; }

        public int CooldownInSeconds { get; set; }

        internal Command CreateCommand()
        {
            return new Command(Shortcut, Name, CooldownInSeconds, Filename, 100);
        }
    }
}