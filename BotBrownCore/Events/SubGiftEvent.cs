using BotBrown.Configuration;

namespace BotBrown.Events
{
    public class SubGiftEvent : UserSpecificEvent
    {
        public SubGiftEvent(ChannelUser user)
            : base(user)
        {
        }
    }
}
