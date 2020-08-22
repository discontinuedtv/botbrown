namespace BotBrownCore.Configuration
{
    using System.Collections.Generic;

    public class CommandConfigurationFileFactory : IConfigurationFileFactory<CommandConfiguration>
    {
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