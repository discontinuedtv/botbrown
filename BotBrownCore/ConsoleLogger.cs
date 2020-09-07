namespace BotBrown
{
    using System;

    public class ConsoleLogger : ILogger
    {
        public void Debug(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH\\:mm\\:ss}][D] {message}");
        }

        public void Error(Exception e)
        {
            Console.WriteLine($"[{DateTime.Now:HH\\:mm\\:ss}][E] {e}");
        }

        public void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH\\:mm\\:ss}][L] {message}");
        }
    }
}