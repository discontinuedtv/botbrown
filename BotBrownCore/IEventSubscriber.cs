namespace BotBrown
{
    using System;

    public interface IEventSubscriber
    {
        Type MessageType { get; }
    }
}