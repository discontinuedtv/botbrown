namespace BotBrownCore.Messaging
{
    using BotBrownCore.Events;
    using System;
    using System.Collections.Concurrent;

    public class Topic<TMessage> : ITopic
        where TMessage : Event
    {
        private readonly ConcurrentDictionary<Guid, ConcurrentQueue<TMessage>> queues = new ConcurrentDictionary<Guid, ConcurrentQueue<TMessage>>();

        public void RegisterConsumer(Guid consumerId)
        {
            if (!queues.TryGetValue(consumerId, out ConcurrentQueue<TMessage> _))
            {
                queues.TryAdd(consumerId, new ConcurrentQueue<TMessage>());
            }
        }

        public void Publish(TMessage message)
        {
            foreach (var queue in queues)
            {
                queue.Value.Enqueue(message);
            }
        }

        public bool TryConsume(Guid consumerId, out TMessage message)
        {
            if (!queues.TryGetValue(consumerId, out ConcurrentQueue<TMessage> queue))
            {
                message = null;
                return false;
            }

            return queue.TryDequeue(out message);
        }
    }
}
