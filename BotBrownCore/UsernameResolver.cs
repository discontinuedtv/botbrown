namespace BotBrown
{
    using BotBrown.Configuration;
    using System.Text;

    public sealed class UsernameResolver : IUsernameResolver
    {
        private readonly IConfigurationManager configurationManager;

        public UsernameResolver(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public ChannelUser ResolveUsername(ChannelUser user)
        {
            UsernameConfiguration usernameConfiguration = configurationManager.LoadConfiguration<UsernameConfiguration>(ConfigurationFileConstants.Usernames);

            if (usernameConfiguration.TryGetValue(user.UserId, out string cachedUsername))
            {
                return user.WithResolvedUsername(cachedUsername);
            }

            StringBuilder sb = new StringBuilder();
            string username = user.RealUsername;

            for (int index = 0; index < username.Length; index++)
            {
                char letter = username[index];

                if (char.IsUpper(letter) && index != 0)
                {
                    sb.Append($" {letter}");
                }
                else if (letter == '_')
                {
                    sb.Append(" ");
                }
                else if (char.IsNumber(letter))
                {
                    continue;
                }
                else
                {
                    sb.Append(letter);
                }
            }

            string targetUsername = sb.ToString();

            ChannelUser userWithResolvedUsername = user.WithResolvedUsername(targetUsername);
            usernameConfiguration.AddUsername(userWithResolvedUsername);
            return userWithResolvedUsername;
        }
    }
}
