using System;
using BotBrownCore.Configuration;

namespace BotBrownCore.Events
{
    public sealed class MessageReceivedEvent : Event
    {
        public MessageReceivedEvent(ChannelUser user, ChatMessage message)
            : base(user)
        {
            Message = message;
        }

        public ChatMessage Message { get; }

        internal void OutputMessage(Action<MessageReceivedEvent> action)
        {
            action(this);
        }
    }
}
