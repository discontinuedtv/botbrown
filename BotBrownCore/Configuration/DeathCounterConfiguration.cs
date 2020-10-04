using Newtonsoft.Json;
using System.Collections.Generic;

namespace BotBrown.Configuration
{
    [ConfigurationFile(ConfigurationFileConstants.DeathCounter)]
    public class DeathCounterConfiguration : IConfiguration
    {
        public Dictionary<string, int> DeathsPerGame { get; set; } = new Dictionary<string, int>();

        [JsonIgnore]
        public string Filename => ConfigurationFileConstants.DeathCounter;

        public bool IsValid()
        {
            return true;
        }
    }
}