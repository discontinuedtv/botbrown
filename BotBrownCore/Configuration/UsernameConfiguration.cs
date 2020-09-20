namespace BotBrown.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public sealed class UsernameConfiguration : IChangeableConfiguration
    {
        public Dictionary<string, ChannelUser> Users { get; set; } = new Dictionary<string, ChannelUser>();
        public event PropertyChangedEventHandler PropertyChanged;

        internal void AddUsername(string userId, string realUsername, string username)
        {
            Users[userId] = new ChannelUser(userId, realUsername, username);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Users)));
        }

        internal bool TryGetValue(string userId, out string username)
        {
            if (Users.TryGetValue(userId, out ChannelUser user))
            {
                username = user.Username;
                return true;
            }

            username = string.Empty;
            return false;
        }

        internal bool FindUserByRealUsername(string realUsername, out ChannelUser user)
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