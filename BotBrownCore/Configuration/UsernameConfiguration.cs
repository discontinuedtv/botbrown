namespace BotBrown.Configuration
{
    using BotBrown;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    [ConfigurationFile(ConfigurationFileConstants.Usernames)]
    public sealed class UsernameConfiguration : IUsernameConfiguration
    {
        public Dictionary<string, ChannelUser> Users { get; set; } = new Dictionary<string, ChannelUser>();

        [JsonIgnore]
        public string Filename => ConfigurationFileConstants.Usernames;

        public event PropertyChangedEventHandler PropertyChanged;

        public void AddUsername(ChannelUser user)
        {
            Users[user.UserId] = user;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Users)));
        }

        public bool TryGetValue(string userId, out string username)
        {
            if (Users.TryGetValue(userId, out ChannelUser user))
            {
                username = user.Username;
                return true;
            }

            username = string.Empty;
            return false;
        }

        public bool FindUserByRealUsername(string realUsername, out ChannelUser user)
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

        public bool IsValid()
        {
            return true;
        }
    }
}