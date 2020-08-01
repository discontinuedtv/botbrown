namespace BotBrownCore.Configuration
{
    using System.Collections.Generic;

    internal class UsernameConfigurationFileFactory : IConfigurationFileFactory<UsernameConfiguration>
    {
        public UsernameConfiguration CreateDefaultConfiguration()
        {
            return new UsernameConfiguration
            {
                Users = new Dictionary<string, ChannelUser>()
            };
        }
    }
}