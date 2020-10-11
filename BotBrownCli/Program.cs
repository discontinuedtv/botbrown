namespace BotBrown
{
    using BotBrown.DI;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            bool dontConnectToTwitch = false;
            bool isDebug = false;
            string customConfigurationPath = null;
            string customSoundsPath = null;
            string logPath = "log/";

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

                if (arg.StartsWith("-ccp:"))
                {
                    customConfigurationPath = arg.Split(':')[1];
                    continue;
                }

                if (arg.StartsWith("-csp:"))
                {
                    customSoundsPath = arg.Split(':')[1];
                    continue;
                }

                if (arg.StartsWith("-l:"))
                {
                    logPath = arg.Split(':')[1];
                }
            }

            var botArguments = new BotArguments(isDebug, dontConnectToTwitch, customConfigurationPath, customSoundsPath, logPath);
            using (var container = new BotContainer(botArguments))
            using (var bot = new Bot())
            {
                bot.Execute(botArguments, container);
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
