namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using System;
    using System.Threading.Tasks;

    public class ViewFactCommand : BaseChatCommand
    {
        private readonly IEventBus eventBus;
        private readonly IConfigurationManager configurationManager;

        public ViewFactCommand(IEventBus eventBus, IConfigurationManager configurationManager)
        {
            this.eventBus = eventBus;
            this.configurationManager = configurationManager;
        }

        public override UserType ElligableUserType => UserType.NonEditors;

        public override string[] Commands => new[] { "fact", "funfact" };

        public override TimeSpan Cooldown => TimeSpan.FromMinutes(1);

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            var facts = configurationManager.LoadConfiguration<FactConfiguration>(ConfigurationFileConstants.Facts);
            if (string.IsNullOrEmpty(chatCommandReceivedEvent.CommandArgs))
            {
                var randomFact = facts.GetRandomFact();

                if (randomFact != null)
                {
                    eventBus.Publish(new SendChannelMessageRequestedEvent(randomFact, chatCommandReceivedEvent.ChannelName));
                }

                return Task.CompletedTask;
            }

            var fact = facts.GetFact(chatCommandReceivedEvent.CommandArgs);
            eventBus.Publish(new SendChannelMessageRequestedEvent(fact, chatCommandReceivedEvent.ChannelName));
            return Task.CompletedTask;
        }
    }
}
