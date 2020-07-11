using System.Collections.Generic;

namespace BotBrownCore
{
    internal class GreetingConfigurationFileFactory : IConfigurationFileFactory<GreetingConfiguration>
    {
        public GreetingConfiguration CreateDefaultConfiguration()
        {
            return new GreetingConfiguration
            {
                Greetings = new Dictionary<string, string>()
            };
        }
    }
}