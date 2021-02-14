namespace BotBrown.ChatCommands
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using Serilog;

    public class AddOrDeleteTargetedCommand : BaseChatCommand
    {
        private readonly IEventBus eventBus;
        private readonly IConfigurationManager configurationManager;
        private readonly ILogger logger;

        public AddOrDeleteTargetedCommand(IEventBus eventBus, IConfigurationManager configurationManager, ILogger logger)
        {
            this.eventBus = eventBus;
            this.configurationManager = configurationManager;
            this.logger = logger.ForContext<AddOrDeleteTargetedCommand>();
        }

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "tcmd+", "tcmd-" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            switch (chatCommandReceivedEvent.CommandText)
            {
                case "tcmd+":
                    AddCommand(chatCommandReceivedEvent);
                    return Task.CompletedTask;
                case "tcmd-":
                    DeleteCommand(chatCommandReceivedEvent);
                    return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        private void DeleteCommand(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            var config = configurationManager.LoadConfiguration<TargetedTextCommandConfiguration>();
            if (config.DeleteCommand(chatCommandReceivedEvent.CommandArgs))
            {
                eventBus.Publish(new SendChannelMessageRequestedEvent(
                    $"Command '{chatCommandReceivedEvent.CommandArgs}' erfolgreich entfernt.",
                    chatCommandReceivedEvent.ChannelName));
                logger.Debug($"Command {chatCommandReceivedEvent.CommandArgs} erfolgreich entfernt.");
            }
            else
            {
                logger.Debug($"Es wurde kein Command {chatCommandReceivedEvent.CommandArgs} gefunden.");
            }
        }

        private void AddCommand(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            if (string.IsNullOrEmpty(chatCommandReceivedEvent.CommandArgs))
            {
                logger.Debug($"Es wurde kein Command eingegeben.");
                return;
            }

            var commandArgs = chatCommandReceivedEvent.CommandArgs.Split(' ');
            if (commandArgs.Length < 2)
            {
                logger.Debug($"Es wurden nicht genug Args für Command eingegeben.");
                return;
            }

            var text = string.Join(" ", commandArgs.Skip(1).ToArray());
            var regex = new Regex(@"\$me|\$[\d]+");
            if (!regex.IsMatch(text))
            {
                logger.Debug($"Command {commandArgs[0]} enthält kein Target und wird nicht angelegt.");
                eventBus.Publish(new SendChannelMessageRequestedEvent(
                    $"Command '{commandArgs[0]}' enthielt kein Target und wird nicht angelegt. Benutzer !+ stattdessen.",
                    chatCommandReceivedEvent.ChannelName));
                return;
            }

            var config = configurationManager.LoadConfiguration<TargetedTextCommandConfiguration>();
            if (config.AddOrUpdateCommand(commandArgs[0], text))
            {
                logger.Debug($"Command {commandArgs[0]} erfolgreich angelegt.");
                eventBus.Publish(new SendChannelMessageRequestedEvent(
                    $"Command '{commandArgs[0]}' erfolgreich angelegt.",
                    chatCommandReceivedEvent.ChannelName));
                return;
            }
        }
    }
}
