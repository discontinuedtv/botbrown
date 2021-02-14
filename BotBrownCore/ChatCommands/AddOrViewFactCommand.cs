namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class AddOrViewFactCommand : BaseChatCommand
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IEventBus eventBus;

        public AddOrViewFactCommand(IConfigurationManager configurationManager, IEventBus eventBus)
        {
            this.configurationManager = configurationManager;
            this.eventBus = eventBus;
        }

        public override UserType ElligableUserType => UserType.AboveSubscriber;

        public override string[] Commands => new[]{ "fact", "funfact" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            var commandArgs = chatCommandReceivedEvent.CommandArgs;
            if(string.IsNullOrEmpty(commandArgs))
            {
                return Task.CompletedTask;
            }

            var facts = configurationManager.LoadConfiguration<FactConfiguration>();
            
            var splittedArgs = commandArgs.Split(' ');
            if(splittedArgs.Length == 1)
            {
                var fact = facts.GetFact(splittedArgs[0]);
                if(fact == null)
                {
                    return Task.CompletedTask;
                }

                eventBus.Publish(new SendChannelMessageRequestedEvent(fact, chatCommandReceivedEvent.ChannelName));
                return Task.CompletedTask;
            }

            var key = splittedArgs[0];
            var newFact = string.Join(" ", splittedArgs.Skip(1).ToArray());
            facts.AddFact(key, newFact);
            eventBus.Publish(new SendChannelMessageRequestedEvent($"Fact zu {key} angelegt / bearbeitet!.", chatCommandReceivedEvent.ChannelName));
            return Task.CompletedTask;
        }
    }
}
