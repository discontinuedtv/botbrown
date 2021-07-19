namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration.Transient;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using System;
    using System.Threading.Tasks;

    public class UnmuteCommand : BaseChatCommand
    {
        private readonly ITransientConfigStore transientConfigStore;
        private readonly IEventBus eventBus;

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "unmute" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => true;

        public UnmuteCommand(ITransientConfigStore transientConfigStore, IEventBus eventBus)
        {
            this.transientConfigStore = transientConfigStore;
            this.eventBus = eventBus;
        }

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            transientConfigStore.Clear<MuteTimer>();
            eventBus.Publish(new SendChannelMessageRequestedEvent("Stummschaltung aufgehoben.", chatCommandReceivedEvent.ChannelName));
            return Task.CompletedTask;
        }
    }
}
