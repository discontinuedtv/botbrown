using System;
using BotBrownCore.Configuration;

namespace BotBrownCore.Events
{
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
