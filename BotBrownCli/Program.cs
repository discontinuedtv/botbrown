namespace BotBrown
{
    using BotBrown.DI;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            BotArguments botArguments = new BotArgumentsFactory().BuildFrom(args);
            using (BotContainer container = new BotContainer(botArguments))
            using (Bot bot = new Bot())
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

            if (theLine != "exit")
            {
                LookForExit(bot);
            }
        }
    }
}
