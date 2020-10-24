﻿namespace BotBrown.Configuration
{
    using System.Collections.Generic;

    public class GreetingConfigurationFileFactory : IConfigurationFileFactory<GreetingConfiguration>
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