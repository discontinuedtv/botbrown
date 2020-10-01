using System.Collections.Generic;

namespace BotBrown.Configuration
{
    public class DeathCounterConfiguration : IConfiguration
    {
        public Dictionary<string, int> DeathsPerGame { get; set; } = new Dictionary<string, int>();
    }
}