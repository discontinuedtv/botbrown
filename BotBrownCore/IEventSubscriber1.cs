using System;

namespace BotBrownCore
{
    public interface IEventSubscriber<TEvent> : IEventSubscriber
    {
        void Handle(TEvent @event);
    }
}