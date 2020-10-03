namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using BotBrown.Workers.Twitch;
    using System;
    using System.Threading.Tasks;
    using Serilog;

    public class IncreaseDeathCommand : BaseChatCommand
    {
        private readonly IEventBus eventBus;
        private readonly IConfigurationManager configurationManager;
        private readonly ITwitchApiWrapper twitchApiWrapper;
        private readonly ILogger logger;

        public IncreaseDeathCommand(
            IEventBus eventBus,
            IConfigurationManager configurationManager,
            ITwitchApiWrapper twitchApiWrapper,
            ILogger logger)
        {
            this.eventBus = eventBus;
            this.configurationManager = configurationManager;
            this.twitchApiWrapper = twitchApiWrapper;
            this.logger = logger.ForContext<IncreaseDeathCommand>();
        }

        public override UserType ElligableUserType => UserType.AboveSubscriber;

        public override string[] Commands => new[] { "death+", "increase-death" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override async Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            var deathConfig = configurationManager.LoadConfiguration<DeathCounterConfiguration>(ConfigurationFileConstants.DeathCounter);
            var game = await twitchApiWrapper.GetCurrentGame();

            if (!string.IsNullOrEmpty(game))
            {
                var newCount = deathConfig.IncreaseDeath(game);
                eventBus.Publish(new SendChannelMessageRequestedEvent($"Oha. Damit sind wir in '{game}' bereits {newCount}-mal gestorben.", chatCommandReceivedEvent.ChannelName));
            }
            else
            {
                logger.Warning("Es konnte kein Spiel ermittelt werden.");
            }
        }
    }
}
