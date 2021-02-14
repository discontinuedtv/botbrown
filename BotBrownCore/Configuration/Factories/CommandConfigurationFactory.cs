namespace BotBrown.Configuration.Factories
{
    using System.Collections.ObjectModel;
    using BotBrown;

    public class CommandConfigurationFileFactory : IConfigurationFileFactory<SoundCommandConfiguration>
    {
        public SoundCommandConfiguration CreateDefaultConfiguration()
        {
            return new SoundCommandConfiguration
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