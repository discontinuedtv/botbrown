﻿namespace BotBrownCore
{
    using System;

    internal class ConsoleLogger : ILogger
    {
        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(Exception e)
        {
            Console.WriteLine(e);
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}