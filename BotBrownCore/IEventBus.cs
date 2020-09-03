namespace BotBrown
{
    using BotBrown.Events;
    using System;

    public interface IEventBus
    {
        void AddTopic<TMessage>()
            where TMessage : Event;

        void SubscribeToTopic<TMessage>(Guid subscriberId)
            where TMessage : Event;

        void Publish<TMessage>(TMessage message)
            where TMessage : Event;

        bool TryConsume<TMessage>(Guid consumerId, out TMessage message)
            where TMessage : Event;
    }
}