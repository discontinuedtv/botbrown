namespace BotBrownCore.Messaging
{
    using BotBrownCore.Events;
    using System;
    using System.Collections.Concurrent;

    public class EventBus : IEventBus
    {
        private object lockObject = new object();
        private ConcurrentDictionary<Type, ITopic> topics = new ConcurrentDictionary<Type, ITopic>();

        public void AddTopic<TMessage>()
            where TMessage : Event
        {
            lock (lockObject)
            {
                if (topics.ContainsKey(typeof(TMessage)))
                {
                    return;
                }

                topics.TryAdd(typeof(TMessage), new Topic<TMessage>());
            }
        }

        public void SubscribeToTopic<TMessage>(Guid subscriberId)
            where TMessage : Event
        {
            lock (lockObject)
            {
                if (!topics.TryGetValue(typeof(TMessage), out ITopic topic))
                {
                    topic = new Topic<TMessage>();
                    topics.TryAdd(typeof(TMessage), topic);
                }

                topic.RegisterConsumer(subscriberId);
            }
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : Event
        {
            if (!topics.TryGetValue(typeof(TMessage), out ITopic topic))
            {
                return;
            }

            ((Topic<TMessage>)topic).Publish(message);
        }

        public bool TryConsume<TMessage>(Guid consumerId, out TMessage message)
            where TMessage : Event
        {
            if (!topics.TryGetValue(typeof(TMessage), out ITopic topic))
            {
                message = null;
                return false;
            }

            return ((Topic<TMessage>)topic).TryConsume(consumerId, out message);
        }
    }
}
