namespace BotBrown.Workers
{
    using BotBrown.Configuration;
    using BotBrown.Workers.TextToSpeech;
    using BotBrown.Workers.Twitch;
    using System.Threading;
    using System.Threading.Tasks;

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

        public void Execute(CancellationToken cancellationToken)
        {
            SpawnTextToSpeechWorker(cancellationToken);
        }

        private void SpawnTextToSpeechWorker(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                var ttsWorker = new TextToSpeechWorker(bus, textToSpeechProcessor);
                return await ttsWorker.Execute(cancellationToken);
            });

            Task.Run(async () =>
            {
                using (var ttsWorker = new TwitchInterfaceWorker(bus, clientWrapper, apiWrapper, logger, configurationManager))
                {
                    return await ttsWorker.Execute(cancellationToken);
                }
            });

            Task.Run(async () =>
            {
                using (var commandWorker = new CommandWorker(bus, configurationManager, presenceStore, textToSpeechProcessor, logger))
                {
                    return await commandWorker.Execute(cancellationToken);
                }
            });
        }
    }
}
