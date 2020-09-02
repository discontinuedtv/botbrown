namespace BotBrown
{
    using System;

    public sealed class DefaultTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
