using System;

namespace BotBrownCore
{
    public class Subscriber<TEvent> : ISubscriber
    {
        private Action<TEvent> action;

        public Subscriber(Action<TEvent> action)
        {
            this.action = action;
        }

        internal void Notify(TEvent @event)
        {
            action(@event);
        }
    }
}