using System;

namespace BotBrown
{
    public interface IEventSubscriber<TEvent> : IEventSubscriber
    {
        void Handle(TEvent @event);
    }
}