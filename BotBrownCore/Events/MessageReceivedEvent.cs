namespace BotBrown.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;    
    using BotBrown;
    using BotBrown.Events.Twitch;

    public sealed class MessageReceivedEvent : UserSpecificEvent
    {
        private readonly List<Emote> emotesInMessage = new List<Emote>();

        public MessageReceivedEvent(ChannelUser user, TwitchChatMessage message, IEnumerable<Emote> emotesInMessage)
            : base(user)
        {
            Message = message;
            this.emotesInMessage.AddRange(emotesInMessage);
        }

        public TwitchChatMessage Message { get; }

        public IReadOnlyCollection<Emote> EmotesInMessage => emotesInMessage.ToList();

        public bool HasEmotesInMessage => emotesInMessage.Count > 0;

        internal void OutputMessage(Action<MessageReceivedEvent> action)
        {
            action(this);
        }
    }
}
