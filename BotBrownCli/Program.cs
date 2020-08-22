namespace BotBrown
{
    using BotBrownCore;
    using BotBrownCore.Configuration;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();

            var apiWrapper = new TwitchApiWrapper();
            var clientWrapper = new TwitchClientWrapper();
            var registry = new ConfigurationFileFactoryRegistry();
            registry.AddFactory(new CommandConfigurationFileFactory());
            registry.AddFactory(new TwitchConfigurationFileFactory());
            registry.AddFactory(new GreetingConfigurationFileFactory());
            registry.AddFactory(new UsernameConfigurationFileFactory());
            registry.AddFactory(new SentenceConfigurationFileFactory());
            registry.AddFactory(new GeneralConfigurationFileFactory());
            var configurationManager = new ConfigurationManager(registry);

            using (var bot = new Bot(apiWrapper, clientWrapper, configurationManager))
            {
                bot.Execute();
                LookForExit(bot);
            }
        }

        private static void LookForExit(Bot bot)
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
