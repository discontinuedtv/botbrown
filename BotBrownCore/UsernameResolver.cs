namespace BotBrown
{
    using BotBrown.Configuration;
    using System.Text;

    public sealed class UsernameResolver : IUsernameResolver
    {
        private readonly IConfigurationManager configurationManager;
        private UsernameConfiguration usernameConfiguration;

        public UsernameResolver(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public ChannelUser ResolveUsername(ChannelUser user)
        {
            usernameConfiguration = configurationManager.LoadConfiguration<UsernameConfiguration>(ConfigurationFileConstants.Usernames);

            if (usernameConfiguration.TryGetValue(user.UserId, out string cachedUsername))
            {
                return new ChannelUser(user.UserId, user.RealUsername, cachedUsername);
            }

            string username = user.RealUsername.Replace("_", " ");
            var sb = new StringBuilder();

            foreach (char c in username)
            {
                if (char.IsUpper(c))
                {
                    sb.Append($" {c}");
                }
                else if (char.IsNumber(c))
                {
                    continue;
                }
                else
                {
                    sb.Append(c);
                }
            }

            string targetUsername = sb.ToString();
            usernameConfiguration.AddUsername(user.UserId, user.RealUsername, targetUsername);
            user.Username = targetUsername;
            configurationManager.WriteConfiguration(usernameConfiguration, ConfigurationFileConstants.Usernames);
            return new ChannelUser(user.UserId, user.RealUsername, targetUsername);
        }
    }
}
