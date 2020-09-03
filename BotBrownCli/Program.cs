namespace BotBrown
{
    using System;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var dontConnectToTwitch = args.Any(x => x == "-notwitch");

            using (var bot = new Bot())
            {
                bot.Execute(dontConnectToTwitch);
                LookForExit(bot);
            }
        }

        private static void LookForExit(Bot bot)
        {
            var theLine = Console.ReadLine();

            if (theLine == "tts")
            {
                bot.PublishTestTTSMessage("Testnachricht");
            }

            if (theLine != "exit")
            {
                LookForExit(bot);
            }
        }
    }
}
