using System;

namespace BotBrownCore
{
    public interface IEventSubscriber
    {
        Type MessageType { get; }
    }
}