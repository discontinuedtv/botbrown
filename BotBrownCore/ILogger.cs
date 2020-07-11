namespace BotBrownCore
{
    using System;

    internal interface ILogger
    {
        void Debug(string message);

        void Error(Exception e);

        void Log(string message);
    }
}