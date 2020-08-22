namespace BotBrownCore
{
    using BotBrownCore.Configuration;
    using System.Collections.Generic;

    internal class PresenceStore
    {
        private readonly HashSet<string> presentUsers = new HashSet<string>();
        private readonly Dictionary<string, int> presencesThisSession = new Dictionary<string, int>();

        internal bool IsGreetingNecessary(ChannelUser user)
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

        internal void RecordPresence(ChannelUser user)
        {
            string userId = user.UserId;
            presentUsers.Add(userId);
        }

        internal bool IsSayByeNecessary(ChannelUser user)
        {
            return presentUsers.Contains(user.UserId);
        }

        internal void RemovePresence(ChannelUser user)
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