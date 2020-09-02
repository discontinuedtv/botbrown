namespace BotBrown
{
    using System;

    public interface ILogger
    {
        void Debug(string message);

        void Error(Exception e);

        void Log(string message);
    }
}