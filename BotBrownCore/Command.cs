namespace BotBrownCore
{
    using System;

    public class Command : IDisposable
    {


        public Command()
        {

        }

        public string Name { get; }

        public string Filename { get; }

        public int CooldownInSeconds { get; }

        public DateTimeOffset Cooldown { get; set; }

        public string Shortcut { get; }

        public void Dispose()
        {
        }

        public void Execute(IBotExecutionContext context)
        {

        }
    }
}