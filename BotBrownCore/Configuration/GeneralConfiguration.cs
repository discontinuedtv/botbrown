namespace BotBrown.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    [ConfigurationFile(ConfigurationFileConstants.General)]
    public class GeneralConfiguration : IConfiguration
    {
        public bool ActivateTextToSpeech { get; set; }

        public string BotChannelGreeting { get; set; }

        public string ByePhrase { get; set; }

        public HashSet<string> ByePhrases { get; set; } = new HashSet<string>();

        public bool IsValid()
        {
            return true;
        }
    }
}
