namespace BotBrown.Events
{
    using System;
    using BotBrown;
    using BotBrown.Events.Twitch;

    public sealed class MessageReceivedEvent : UserSpecificEvent
    {
        public MessageReceivedEvent(ChannelUser user, TwitchChatMessage message)
            : base(user)
        {
            Message = message;
        }

        public TwitchChatMessage Message { get; }

        internal void OutputMessage(Action<MessageReceivedEvent> action)
        {
            action(this);
        }
    }
}
