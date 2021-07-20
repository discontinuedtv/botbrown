namespace BotBrown.Workers.Twitch
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TwitchLib.Api;
    using TwitchLib.Api.Core;
    using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
    using TwitchLib.Api.Services;
    using TwitchLib.Api.Services.Events.FollowerService;

    public class TwitchApiWrapper : ITwitchApiWrapper
    {
        private readonly IUsernameResolver usernameResolver;
        private readonly IEventBus bus;
        private readonly IConfigurationManager configurationManager;
        private FollowerService followerService;
        private TwitchAPI api;

        public TwitchApiWrapper(IUsernameResolver usernameResolver, IEventBus bus, IConfigurationManager configurationManager)
        {
            this.usernameResolver = usernameResolver;
            this.bus = bus;
            this.configurationManager = configurationManager;
        }

        public void ConnectToTwitch(TwitchConfiguration twitchConfiguration)
        {
            SetUpEvents();

            api = InitializeTwitchApi(twitchConfiguration);
            CreateFollowerService(twitchConfiguration, api);
            RegisterFollowerServiceCallback();
            followerService.Start();
        }

        private void SetUpEvents()
        {
            bus.AddTopic<NewFollowerEvent>();
        }

        public void Stop()
        {
            followerService?.Stop();
        }

        private TwitchAPI InitializeTwitchApi(TwitchConfiguration twitchConfiguration)
        {
            return new TwitchAPI(null, null, new ApiSettings
            {
                ClientId = twitchConfiguration.ApiClientId,
                AccessToken = twitchConfiguration.ApiAccessToken
            });
        }

        private void CreateFollowerService(TwitchConfiguration twitchConfiguration, TwitchAPI api)
        {
            followerService = new FollowerService(api, 10);
            followerService.SetChannelsByName(new List<string> { twitchConfiguration.Username }); // TODO ändern!
        }

        private void RegisterFollowerServiceCallback()
        {
            followerService.OnNewFollowersDetected += Api_OnFollowerDetected;
        }

        private void Api_OnFollowerDetected(object sender, OnNewFollowersDetectedArgs newFollowerDetectedEventArguments)
        {
            DateTime dateToCheckAgainst = DateTime.UtcNow.AddSeconds(-90);

            List<ChannelUser> newFollowers = newFollowerDetectedEventArguments.NewFollowers
                .Where(follow => follow.FollowedAt >= dateToCheckAgainst)
                .Select(follow => usernameResolver.ResolveUsername(new ChannelUser(follow.FromUserId, follow.FromUserName, follow.FromUserName)))
                .ToList();

            NewFollowerEvent newFollowerEvent = new NewFollowerEvent(newFollowers);
            bus.Publish(newFollowerEvent);
        }

        public async Task UpdateChannel(UpdateChannelEvent updateChannelEvent)
        {
            TwitchConfiguration twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>();
            var channelInformationResponse = await api.Helix.Channels.GetChannelInformationAsync(twitchConfiguration.BroadcasterUserId);

            var channelInformation = channelInformationResponse.Data.FirstOrDefault();
            if (channelInformation == null)
            {
                return;
            }

            updateChannelEvent.Update(channelInformation);

            var gamesResponse = await api.Helix.Games.GetGamesAsync(null, new List<string> { updateChannelEvent.Game });
            var game = gamesResponse.Games.FirstOrDefault();

            var modifyChannelInformationRequest = new ModifyChannelInformationRequest
            {
                Title = updateChannelEvent.Title,
                GameId = game?.Id ?? channelInformation.GameId
            };

            await api.Helix.Channels.ModifyChannelInformationAsync(twitchConfiguration.BroadcasterUserId, modifyChannelInformationRequest, twitchConfiguration.AccessToken);
            bus.Publish(new SendChannelMessageRequestedEvent("Titel oder Game wurden geändert.", twitchConfiguration.Channel));
        }

        public async Task<string> GetCurrentGame()
        {
            TwitchConfiguration twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>();
            var channel = await api.V5.Channels.GetChannelByIDAsync(twitchConfiguration.BroadcasterUserId);
            return channel.Game;
        }

        public async Task<string> GetUserIdByUsername(string username)
        {
            var twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>();
            var usersResponse = await api.Helix.Users.GetUsersAsync(null, new List<string> { username }, twitchConfiguration.AccessToken);


            if (usersResponse.Users.Length != 1)
            {
                return null;
            }

            return usersResponse.Users.FirstOrDefault()?.Id;
        }
    }
}
