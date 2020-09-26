namespace BotBrown.Configuration
{
    using System.Collections.Generic;

    public class GeneralConfiguration : IConfiguration
    {
        public bool ActivateTextToSpeech { get; set; }

        public string BotChannelGreeting { get; set; }

        public string ByePhrase { get; set; }

        public HashSet<string> ByePhrases { get; set; } = new HashSet<string>();
    }
}
