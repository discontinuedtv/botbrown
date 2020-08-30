using System;

namespace BotBrownCore.Messaging
{
    public interface ITopic
    {
        void RegisterConsumer(Guid subscriberId);
    }
}