namespace BotBrown.ChatCommands
{
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using BotBrown.Workers.TextToSpeech;
    using System;
    using System.Threading.Tasks;

    public class GetLanguagesCommand : BaseChatCommand
    {
        private readonly ITextToSpeechProcessor textToSpeechProcessor;
        private readonly IEventBus eventBus;

        public GetLanguagesCommand(ITextToSpeechProcessor textToSpeechProcessor, IEventBus eventBus)
        {
            this.textToSpeechProcessor = textToSpeechProcessor;
            this.eventBus = eventBus;
        }

        public override UserType ElligableUserType => UserType.All;

        public override string[] Commands => new[] { "sprachen" };

        public override TimeSpan Cooldown => TimeSpan.FromSeconds(10);

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            string languages = textToSpeechProcessor.TextToSpeechLanguages;
            eventBus.Publish(new SendWhisperMessageRequestedEvent(chatCommandReceivedEvent.User, languages));
            return Task.CompletedTask;
        }
    }
}
