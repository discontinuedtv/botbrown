namespace BotBrown.Configuration
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    [ConfigurationFile(ConfigurationFileConstants.General)]
    public class GeneralConfiguration : IConfiguration
    {
        public bool ActivateTextToSpeech { get; set; }

        public string BotChannelGreeting { get; set; }

        public string ByePhrase { get; set; }

        public HashSet<string> ByePhrases { get; set; } = new HashSet<string>();

        [JsonIgnore]
        public string Filename => ConfigurationFileConstants.General;

        public bool IsValid()
        {
            return true;
        }
    }
}
