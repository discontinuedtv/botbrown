using System;

namespace BotBrown
{
    public interface IEventSubscriber
    {
        Type MessageType { get; }
    }
}