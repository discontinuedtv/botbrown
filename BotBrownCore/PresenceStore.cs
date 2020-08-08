namespace BotBrownCore
{
    using System.Collections.Generic;

    internal class PresenceStore
    {
        private readonly HashSet<string> presentUsers = new HashSet<string>();
        private readonly Dictionary<string, int> presencesThisSession = new Dictionary<string, int>();

        internal bool IsGreetingNecessary(string userId)
        {
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

        internal void RecordPresence(string userId)
        {
            presentUsers.Add(userId);
        }

        internal bool IsSayByeNecessary(string userId)
        {
            return presentUsers.Contains(userId);
        }

        internal void RemovePresence(string userId)
        {
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