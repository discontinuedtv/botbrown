namespace BotBrown
{
    using System;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            bool dontConnectToTwitch = false;
            bool isDebug = false;

            foreach (var arg in args)
            {
                if (arg == "-notwitch")
                {
                    dontConnectToTwitch = true;
                    continue;
                }

                if (arg == "-debug")
                {
                    isDebug = true;
                    continue;
                }
            }

            var botArguments = new BotArguments(isDebug, dontConnectToTwitch);

            using (var bot = new Bot())
            {
                bot.Execute(botArguments);
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

            if (theLine == "cmd")
            {
                bot.PublishSoundCommand("scoddiNice");
            }

            if (theLine != "exit")
            {
                LookForExit(bot);
            }
        }
    }
}
