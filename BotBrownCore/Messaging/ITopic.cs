using System;

namespace BotBrown.Messaging
{
    public interface ITopic
    {
        void RegisterConsumer(Guid subscriberId);
    }
}