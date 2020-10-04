namespace BotBrown.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    [ConfigurationFile(ConfigurationFileConstants.General)]
    public class GeneralConfiguration : IChangeableConfiguration
    {
        public bool ActivateTextToSpeech { get; set; }

        public string BotChannelGreeting { get; set; }

        public string ByePhrase { get; set; }

        public HashSet<string> ByePhrases { get; set; } = new HashSet<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Changed()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GeneralConfiguration)));
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
