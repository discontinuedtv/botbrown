namespace BotBrown.Configuration.Factories
{
    using System.Collections.ObjectModel;
    using BotBrown;

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