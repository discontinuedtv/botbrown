namespace BotBrown
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            using (var bot = new BotBrownCore.Bot())
            {
                Console.ReadLine();
            }
        }
    }
}
