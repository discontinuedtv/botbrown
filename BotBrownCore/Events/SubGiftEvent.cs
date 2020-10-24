namespace BotBrown.Events
{
    using BotBrown;

    public class SubGiftEvent : UserSpecificEvent
    {
        public SubGiftEvent(ChannelUser user)
            : base(user)
        {
        }
    }
}
