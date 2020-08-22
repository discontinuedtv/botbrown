namespace BotBrownCore.Events
{
    using BotBrownCore.Configuration;
    using System.Collections.Generic;

    public class NewFollowerEvent
    {
        private List<ChannelUser> newFollowers = new List<ChannelUser>();

        public IReadOnlyList<ChannelUser> NewFollowers
        {
            get
            {
                return newFollowers;
            }
        }

        public NewFollowerEvent(IEnumerable<ChannelUser> newFollowers)
        {
            foreach (ChannelUser newFollower in newFollowers)
            {
                this.newFollowers.Add(newFollower);
            }
        }
    }
}
