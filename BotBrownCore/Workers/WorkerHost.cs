namespace BotBrown.Workers
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Workers.TextToSpeech;
    using BotBrown.Workers.Twitch;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Owin.Hosting;

    public class WorkerHost : IWorkerHost
    {
        private readonly IEventBus bus;
        private readonly ITextToSpeechProcessor textToSpeechProcessor;
        private readonly ITwitchClientWrapper clientWrapper;
        private readonly ITwitchApiWrapper apiWrapper;
        private readonly ILogger logger;
        private readonly IConfigurationManager configurationManager;
        private readonly IPresenceStore presenceStore;

        public WorkerHost(IEventBus bus, ITextToSpeechProcessor textToSpeechProcessor, ITwitchClientWrapper clientWrapper, ITwitchApiWrapper apiWrapper, ILogger logger, IConfigurationManager configurationManager, IPresenceStore presenceStore)
        {
            this.bus = bus;
            this.textToSpeechProcessor = textToSpeechProcessor;
            this.clientWrapper = clientWrapper;
            this.apiWrapper = apiWrapper;
            this.logger = logger;
            this.configurationManager = configurationManager;
            this.presenceStore = presenceStore;
        }

        public void Execute(CancellationToken cancellationToken, bool dontConnectToTwitch)
        {
            SpawnWorkerTasks(cancellationToken, dontConnectToTwitch);
        }

        public void PublishTTSMessage(string message)
        {
            bus.Publish<TextToSpeechEvent>(new SpeakEvent(new ChannelUser("46409199", "discontinuedman", "discontinjudmän"), message));
        }

        public void PublishSoundCommand(string message)
        {
            bus.Publish(new PlaySoundRequestedEvent(message));
        }

        private void SpawnWorkerTasks(CancellationToken cancellationToken, bool dontConnectToTwitch)
        {
            Task.Run(async () =>
            {
                var ttsWorker = new TextToSpeechWorker(bus, textToSpeechProcessor);
                return await ttsWorker.Execute(cancellationToken);
            });

            SpawnTwitchWorker(cancellationToken, dontConnectToTwitch);

            Task.Run(async () =>
            {
                using (var commandWorker = new CommandWorker(bus, configurationManager, presenceStore, textToSpeechProcessor, logger))
                {
                    return await commandWorker.Execute(cancellationToken);
                }
            });

            SpawnWebserver(cancellationToken);
        }

        private static void SpawnWebserver(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                using (WebApp.Start<WebserverStartup>("http://localhost:12345"))
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(1000);
                    }
                }
            });
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