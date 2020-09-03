namespace BotBrown
{
    using System;

    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}
