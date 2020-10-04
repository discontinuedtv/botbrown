namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using System;
    using System.Threading.Tasks;

    public class DeleteFactCommand : BaseChatCommand
    {
        private readonly IEventBus eventBus;
        private readonly IConfigurationManager configurationManager;

        public DeleteFactCommand(IEventBus eventBus, IConfigurationManager configurationManager)
        {
            this.eventBus = eventBus;
            this.configurationManager = configurationManager;
        }

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "delfact", "delfunfact" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            var facts = configurationManager.LoadConfiguration<FactConfiguration>(ConfigurationFileConstants.Facts);
            facts.RemoveFact(chatCommandReceivedEvent.CommandArgs);
            eventBus.Publish(
                new SendChannelMessageRequestedEvent($"Fakt zu {chatCommandReceivedEvent.CommandArgs} entfernt.", chatCommandReceivedEvent.ChannelName));
            return Task.CompletedTask;
        }
    }
}
