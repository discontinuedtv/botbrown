using BotBrownCore.Configuration;

namespace BotBrownCore.Events
{
    public class SubGiftEvent : UserSpecificEvent
    {
        public SubGiftEvent(ChannelUser user)
            : base(user)
        {
        }
    }
}
