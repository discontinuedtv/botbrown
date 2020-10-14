namespace BotBrown.ChatCommands
{
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using System;
    using System.Threading.Tasks;

    public class WhatsTheTimeCommand : BaseChatCommand
    {
        private readonly IEventBus bus;

        public WhatsTheTimeCommand(IEventBus bus)
        {
            this.bus = bus;
        }

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "time" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            DateTime now = DateTime.Now;
            bus.Publish(new TextToSpeechEvent(chatCommandReceivedEvent.User, $"Die Zeitleitung zeigt {now:HH} Uhr {now:mm} an."));
            return Task.CompletedTask;
        }
    }
}
