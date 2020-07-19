namespace BotBrownCore.Configuration
{
    using System.Collections.Generic;

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