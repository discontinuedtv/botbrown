using System.Collections.Generic;

namespace BotBrownCore
{
    internal class CommandConfigurationFileFactory : IConfigurationFileFactory<CommandConfiguration>
    {
        public CommandConfigurationFileFactory()
        {
        }

        public CommandConfiguration CreateDefaultConfiguration()
        {
            return new CommandConfiguration
            {
                CommandsDefinitions = new List<CommandDefinition>
                {
                    new CommandDefinition
                    {
                        Shortcut = "scoddiNice",
                        Name = "NICE",
                        Filename = "clickNice.wav",
                        CooldownInSeconds = 60
                    }
                }
            };
        }
    }
}