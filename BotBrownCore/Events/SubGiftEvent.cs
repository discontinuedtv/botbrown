using BotBrownCore.Configuration;

namespace BotBrownCore.Events
{
    public class SubGiftEvent : Event
    {
        public SubGiftEvent(ChannelUser user)
            : base(user)
        {
        }
    }
}
