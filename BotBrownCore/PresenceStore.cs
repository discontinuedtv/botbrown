namespace BotBrown
{
    using BotBrown.Configuration;
    using System.Collections.Generic;

    public class PresenceStore : IPresenceStore
    {
        private readonly HashSet<string> presentUsers = new HashSet<string>();
        private readonly Dictionary<string, int> presencesThisSession = new Dictionary<string, int>();

        public bool IsGreetingNecessary(ChannelUser user)
        {
            string userId = user.UserId;

            if (!presentUsers.Contains(userId))
            {
                if (!presencesThisSession.TryGetValue(userId, out int presences))
                {
                    return true;
                }

                return presences < 1;
            }

            return false;
        }

        public void RecordPresence(ChannelUser user)
        {
            string userId = user.UserId;
            presentUsers.Add(userId);
        }

        public bool IsSayByeNecessary(ChannelUser user)
        {
            return presentUsers.Contains(user.UserId);
        }

        public void RemovePresence(ChannelUser user)
        {
            string userId = user.UserId;

            presentUsers.Remove(userId);
            
            if (!presencesThisSession.ContainsKey(userId))
            {
                presencesThisSession[userId] = 1;
            }
            else
            {
                presencesThisSession[userId]++;
            }
        }
    }
}