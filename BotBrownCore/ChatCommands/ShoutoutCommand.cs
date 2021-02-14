namespace BotBrown.ChatCommands
{
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using System;
    using System.Threading.Tasks;

    public class ShoutoutCommand : BaseChatCommand
    {
        private readonly IEventBus eventBus;

        public ShoutoutCommand(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "so", "shoutout" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            // tbd: This could be enriched with logo and other informations of the streamer if this should be shown on screen

            var args = chatCommandReceivedEvent.CommandArgs;
            if (args.StartsWith("@"))
            {
                args = args.Substring(1);
            }

            eventBus.Publish(new SendChannelMessageRequestedEvent(
                $"Schaut euch auch {args} an und lasst eventuell einen Follow da: https://twitch.tv/{args}",
                chatCommandReceivedEvent.ChannelName));
            return Task.CompletedTask;
        }
    }
}
