namespace BotBrown.Configuration.Factories
{
    using BotBrown;
    using System.Collections.Generic;

    public class UsernameConfigurationFileFactory : IConfigurationFileFactory<UsernameConfiguration>
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