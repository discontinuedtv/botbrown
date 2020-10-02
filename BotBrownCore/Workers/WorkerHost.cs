namespace BotBrown.Workers
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Workers.TextToSpeech;
    using BotBrown.Workers.Twitch;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Owin.Hosting;
    using System;
    using Owin;
    using BotBrown.Workers.Webserver;
    using Castle.Windsor;
    using BotBrown;
    using BotBrown.ChatCommands;
    using Serilog;

    public class WorkerHost : IWorkerHost
    {
        private readonly IEventBus bus;
        private readonly ITextToSpeechProcessor textToSpeechProcessor;
        private readonly ITwitchClientWrapper clientWrapper;
        private readonly ITwitchApiWrapper apiWrapper;
        private readonly ILogger logger;
        private readonly IConfigurationManager configurationManager;
        private readonly IPresenceStore presenceStore;
        private readonly IChatCommandResolver chatCommandResolver;
        private readonly ISoundProcessor soundProcessor;

        public WorkerHost(
            IEventBus bus,
            ITextToSpeechProcessor textToSpeechProcessor,
            ITwitchClientWrapper clientWrapper,
            ITwitchApiWrapper apiWrapper,
            ILogger logger,
            IConfigurationManager configurationManager,
            IPresenceStore presenceStore,
            IChatCommandResolver chatCommandResolver,
            ISoundProcessor soundProcessor)
        {
            this.bus = bus;
            this.textToSpeechProcessor = textToSpeechProcessor;
            this.clientWrapper = clientWrapper;
            this.apiWrapper = apiWrapper;
            this.logger = logger.ForContext<WorkerHost>();
            this.configurationManager = configurationManager;
            this.presenceStore = presenceStore;
            this.chatCommandResolver = chatCommandResolver;
            this.soundProcessor = soundProcessor;
        }

        public WindsorContainer Container { get; set; }

        public void Execute(CancellationToken cancellationToken, BotArguments botArguments)
        {
            SpawnWorkerTasks(cancellationToken, botArguments);
        }

        public void PublishTTSMessage(string message)
        {
            bus.Publish<TextToSpeechEvent>(new SpeakEvent(new ChannelUser("46409199", "discontinuedman", "discontinjudmän"), message));
        }

        public void PublishSoundCommand(string message)
        {
            bus.Publish(new PlaySoundRequestedEvent(message));
        }

        private void SpawnWorkerTasks(CancellationToken cancellationToken, BotArguments botArguments)
        {
            SpawnTTSWorker(cancellationToken);
            SpawnTwitchWorker(cancellationToken, botArguments.DontConnectToTwitch);
            SpawnCommandWorker(cancellationToken);
            SpawnWebserver(cancellationToken, botArguments.IsDebug, botArguments.Port);
            SpawnConfigurationWatcher(cancellationToken);
        }

        private void SpawnConfigurationWatcher(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                var configurationWatcher = new ConfigurationWatcher(configurationManager);
                return await configurationWatcher.StartWatch(cancellationToken);
            });
        }

        private void SpawnTTSWorker(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                var ttsWorker = new SoundWorker(bus, textToSpeechProcessor, soundProcessor, logger);
                return await ttsWorker.Execute(cancellationToken);
            });
        }

        private void SpawnCommandWorker(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                using (var commandWorker = new CommandWorker(bus, configurationManager, presenceStore, textToSpeechProcessor, logger, chatCommandResolver))
                {
                    return await commandWorker.Execute(cancellationToken);
                }
            });
        }

        private void SpawnWebserver(CancellationToken cancellationToken, bool isDebug, string portOverwrite)
        {
            Task.Run(async () =>
            {
                string port = portOverwrite == null ? (isDebug ? WebserverConstants.DebugPort : WebserverConstants.ProductivePort) : portOverwrite;
                string webserverUrl = $"http://localhost:{port}";

                using (WebApp.Start(webserverUrl, CreateStartup))
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(1000);
                    }
                }
            });
        }

        private void CreateStartup(IAppBuilder appBuilder)
        {
            if (Container == null)
            {
                throw new InvalidOperationException("Kein CONTAINER!!");
            }

            var startup = new WebserverStartup(Container);
            startup.Configuration(appBuilder);
        }

        private void SpawnTwitchWorker(CancellationToken cancellationToken, bool dontConnectToTwitch)
        {
            if (dontConnectToTwitch)
            {
                return;
            }

            Task.Run(async () =>
            {
                using (var ttsWorker = new TwitchInterfaceWorker(bus, clientWrapper, apiWrapper, logger, configurationManager))
                {
                    return await ttsWorker.Execute(cancellationToken);
                }
            });
        }
    }
}