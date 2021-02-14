namespace BotBrown.Messaging
{
    using System;

    public interface ITopic
    {
        void RegisterConsumer(Guid subscriberId);
    }
}