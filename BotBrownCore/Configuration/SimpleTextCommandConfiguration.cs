namespace BotBrown.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [ConfigurationFile(ConfigurationFileConstants.TextCommands)]
    public class SimpleTextCommandConfiguration : IChangeableConfiguration
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<string, string> Commands { get; set; } = new Dictionary<string, string>();

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

        internal void ProcessMessage(string message)
        {
            var indicator = message.Substring(0, 2);
            var splittedString = message.Substring(2).Split(' ');
            var command = splittedString[0];

            switch (indicator)
            {
                case "+!":
                    AddOrUpdateCommand(command, string.Join(" ", splittedString.Skip(1)));
                    return;
                case "-!":
                    DeleteCommand(command);
                    return;
            }
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
