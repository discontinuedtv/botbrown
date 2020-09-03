namespace BotBrown
{
    using BotBrown.Configuration;
    using BotBrown.Messaging;
    using BotBrown.Workers;
    using BotBrown.Workers.TextToSpeech;
    using BotBrown.Workers.Twitch;
    using System;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            using (var bot = new Bot())
            {
                bot.Execute();
                LookForExit(bot);
            }
        }

        private static void LookForExit(Bot bot)
        {
            var theLine = Console.ReadLine();

            if (theLine != "exit")
            {
                LookForExit(bot);
            }
        }
    }
}
