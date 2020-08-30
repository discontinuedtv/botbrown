namespace BotBrownCore
{
    using System;

    public sealed class DefaultTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
