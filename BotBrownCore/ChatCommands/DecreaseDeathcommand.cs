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

    public class DecreaseDeathCommand : BaseChatCommand
    {
        private readonly IEventBus eventBus;
        private readonly IConfigurationManager configurationManager;
        private readonly ITwitchApiWrapper twitchApiWrapper;
        private readonly ILogger logger;

        public DecreaseDeathCommand(
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

        public override string[] Commands => new[] { "death-", "decrease-death" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override async Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            var deathConfig = configurationManager.LoadConfiguration<DeathCounterConfiguration>();
            var game = await twitchApiWrapper.GetCurrentGame();
            var channelName = chatCommandReceivedEvent.ChannelName;

            if (!string.IsNullOrEmpty(game))
            {
                var newCount = deathConfig.DecreaseDeath(game);
                if (newCount == 0)
                {
                    eventBus.Publish(new SendChannelMessageRequestedEvent($"Im Spiel {game} sind wir damit bisher noch nie gestorben.", channelName));
                }
                else
                {
                    eventBus.Publish(new SendChannelMessageRequestedEvent($"Oha. Damit sind wir in '{game}' nur {newCount}-mal gestorben.", channelName));
                }
            }
            else
            {
                logger.Warning("Es konnte kein Spiel ermittelt werden.");
            }
        }
    }
}
