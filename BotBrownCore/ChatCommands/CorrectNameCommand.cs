namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using System;
    using System.Threading.Tasks;

    public class CorrectNameCommand : BaseChatCommand
    {
        private readonly IConfigurationManager configurationManager;

        public CorrectNameCommand(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "correctname" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            string[] namechangeParameters = chatCommandReceivedEvent.CommandArgs.Split(';');
            if (namechangeParameters.Length != 2)
            {
                return Task.CompletedTask;
            }

            var usernameConfiguration = configurationManager.LoadConfiguration<UsernameConfiguration>();
            if (!usernameConfiguration.FindUserByRealUsername(namechangeParameters[0], out ChannelUser user))
            {
                return Task.CompletedTask;
            }

            usernameConfiguration.AddUsername(user.WithResolvedUsername(namechangeParameters[1]));
            return Task.CompletedTask;
        }
    }
}
