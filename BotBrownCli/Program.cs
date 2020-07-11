namespace BotBrown
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            using (var bot = new BotBrownCore.Bot())
            {
                bot.Execute();
                LookForExit(bot);
            }
        }

        private static void LookForExit(BotBrownCore.Bot bot)
        {
            var theLine = Console.ReadLine();

            if (theLine == "refresh")
            {
                bot.RefreshCommands();
            }

            if (theLine != "exit")
            {
                LookForExit(bot);
            }
        }
    }
}
