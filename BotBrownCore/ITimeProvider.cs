namespace BotBrownCore
{
    using System;

    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}
