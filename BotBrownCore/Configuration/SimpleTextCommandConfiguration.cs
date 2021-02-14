namespace BotBrown.Configuration
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel;

    [ConfigurationFile(ConfigurationFileConstants.TextCommands)]
    public class SimpleTextCommandConfiguration : IChangeableConfiguration
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<string, string> Commands { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public string Filename => ConfigurationFileConstants.TextCommands;

        public void AddOrUpdateCommand(string command, string commandText)
        {
            Commands[command] = commandText;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Commands)));
        }

        public void DeleteCommand(string command)
        {
            Commands.Remove(command);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Commands)));
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
