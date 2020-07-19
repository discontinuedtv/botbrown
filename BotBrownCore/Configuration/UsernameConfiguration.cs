namespace BotBrownCore.Configuration
{
    using System;
    using System.Collections.Generic;

    public sealed class UsernameConfiguration : IConfiguration
    {
        public Dictionary<string, User> Users { get; set; } = new Dictionary<string, User>();

        internal void AddUsername(string userId, string realUsername, string username)
        {
            Users[userId] = new User(userId, realUsername, username);
        }

        internal bool TryGetValue(string userId, out string username)
        {
            if (Users.TryGetValue(userId, out User user))
            {
                username = user.Username;
                return true;
            }

            username = string.Empty;
            return false;
        }

        internal bool FindUserByRealUsername(string realUsername, out User user)
        {
            foreach (var userEntry in Users)
            {
                if (userEntry.Value.RealUsername.Equals(realUsername, StringComparison.OrdinalIgnoreCase))
                {
                    user = userEntry.Value;
                    return true;
                }
            }

            user = null;
            return false;
        }
    }
}