namespace BotBrown
{
    using System;

    public interface IEventSubscriber<TEvent> : IEventSubscriber
    {
        void Handle(TEvent @event);
    }
}