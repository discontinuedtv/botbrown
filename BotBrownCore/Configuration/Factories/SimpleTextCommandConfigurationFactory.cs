﻿namespace BotBrown.Configuration.Factories
{
    public class SimpleTextCommandConfigurationFactory : IConfigurationFileFactory<SimpleTextCommandConfiguration>
    {
        public SimpleTextCommandConfiguration CreateDefaultConfiguration()
        {
            return new SimpleTextCommandConfiguration
            {
                Commands = new System.Collections.Generic.Dictionary<string, string>
                {
                    {
                       "botbrown","Der Bot ist hier zu finden: https://github.com/discontinuedtv/botbrown"
                    }
                }
            };
        }
    }
}
