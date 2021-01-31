namespace BotBrown.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Newtonsoft.Json;

    [ConfigurationFile(ConfigurationFileConstants.TargetedTextCommands)]
    public class TargetedTextCommandConfiguration : IChangeableConfiguration
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<string, string> Commands { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public string Filename => ConfigurationFileConstants.TextCommands;

        public bool AddOrUpdateCommand(string command, string commandText)
        {
            if (string.IsNullOrEmpty(command))
            {
                return false;
            }

            Commands[command] = commandText;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Commands)));
            return true;
        }

        public bool DeleteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return false;
            }

            if (Commands.Remove(command))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Commands)));
                return true;
            }

            return false;
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
