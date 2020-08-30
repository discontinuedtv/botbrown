namespace BotBrown
{
    using BotBrownCore;
    using BotBrownCore.Configuration;
    using BotBrownCore.Messaging;
    using BotBrownCore.Workers;
    using BotBrownCore.Workers.TextToSpeech;
    using BotBrownCore.Workers.Twitch;
    using System;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                IEventBus bus = new EventBus();

                var registry = new ConfigurationFileFactoryRegistry();
                registry.AddFactory(new CommandConfigurationFileFactory());
                registry.AddFactory(new TwitchConfigurationFileFactory());
                registry.AddFactory(new GreetingConfigurationFileFactory());
                registry.AddFactory(new UsernameConfigurationFileFactory());
                registry.AddFactory(new SentenceConfigurationFileFactory());
                registry.AddFactory(new GeneralConfigurationFileFactory());
                var configurationManager = new ConfigurationManager(registry);

                var logger = new ConsoleLogger();

                var usernameResolver = new UsernameResolver(configurationManager);
                var apiWrapper = new TwitchApiWrapper(usernameResolver, bus);
                var clientWrapper = new TwitchClientWrapper(usernameResolver, bus, logger);
                var textToSpeechProcessor = new TextToSpeechProcessor(configurationManager);
                                
                var presenceStore = new PresenceStore();

                var workerHost = new WorkerHost(bus, textToSpeechProcessor, clientWrapper, apiWrapper, logger, configurationManager, presenceStore);
                workerHost.Execute(cancellationTokenSource.Token);

                using (var bot = new Bot())
                {
                    bot.Execute();
                    LookForExit(bot, cancellationTokenSource);
                }
            }
        }

        private static void LookForExit(Bot bot, CancellationTokenSource tokenSource)
        {
            var theLine = Console.ReadLine();

            if (theLine != "exit")
            {
                LookForExit(bot, tokenSource);
            }
            else
            {
                tokenSource.Cancel();
            }
        }
    }
}
