using System.Collections.Generic;
using System.ComponentModel;

namespace BotBrown.Configuration
{
    public class GeneralConfiguration : IConfiguration
    {
        public bool ActivateTextToSpeech { get; set; }

        public string BotChannelGreeting { get; set; }

        public string ByePhrase { get; set; }

        public HashSet<string> ByePhrases { get; set; } = new HashSet<string>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
