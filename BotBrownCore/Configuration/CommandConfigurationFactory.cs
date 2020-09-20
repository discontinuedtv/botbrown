namespace BotBrown.Configuration
{
    using System.Collections.ObjectModel;

    public class CommandConfigurationFileFactory : IConfigurationFileFactory<CommandConfiguration>
    {
        public CommandConfiguration CreateDefaultConfiguration()
        {
            return new CommandConfiguration
            {
                CommandsDefinitions = new ObservableCollection<CommandDefinition>
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