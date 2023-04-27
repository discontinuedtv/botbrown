namespace BotbrownWPF.Views
{
    using BotBrown.Configuration;
    using System;
    using System.Collections.Generic;

    public class WebViewModel
    {
        public string AccessToken { get; set; }

        public TwitchUserInfo UserInfo { get; set; }

        public string ChannelName { get; set; }

        public Uri TargetUri 
        {
            get
            {
                List<string> scopes = new List<string>();
                scopes.Add("channel_editor");
                scopes.Add("channel_subscriptions");

                scopes.Add("chat:read"); // View live Stream Chat and Rooms messages
                scopes.Add("chat:edit"); // Send live Stream Chat and Rooms messages
                scopes.Add("channel:moderate"); // Perform moderation actions in a channel
                scopes.Add("channel:read:subscriptions"); // Get a list of all subscribers to your channel and check if a user is subscribed to your channel
                scopes.Add("whispers:read"); // View your whisper messages.
                scopes.Add("whispers:edit"); // Send whisper messages.
                scopes.Add("channel:manage:broadcast"); // Manage your channel's broadcast configuration, including updating channel configuration and managing stream markers and stream tags.
                scopes.Add("user:edit:broadcast"); // Edit your channel's broadcast configuration, including extension configuration. (This scope implies user:read:broadcast capability.)
                scopes.Add("user:read:broadcast"); // View your broadcasting configuration, including extension configurations.

                // scopes.Add("channel_check_subscription") //	Read access to check if a user is subscribed to your channel.
                // scopes.Add("channel_feed_read"); // Ability to view a channel feed.
                // scopes.Add("channel_feed_edit"); // Ability to add posts and reactions to a channel feed.
                // scopes.Add("collections_edit"); // Manage a user's collections (of videos).
                // scopes.Add("communities_edit"); // Manage a user's communities.
                // scopes.Add("communities_moderate"); // Manage community moderators.
                // scopes.Add("viewing_activity_read"); // Turn on Viewer Heartbeat Service ability to record user data.
                // scopes.Add("openid"); // Use OpenID Connect authentication.
                // scopes.Add("analytics:read:extensions"); // View analytics data for your extensions.
                // scopes.Add("user:edit");	// Manage a user object.
                // scopes.Add("user:read:email"); // Read authorized user's email address.
                // scopes.Add("clips:edit"); // Create and edit clips as a specific user.
                // scopes.Add("bits:read"); // View bits information for your channel.
                // scopes.Add("analytics:read:games"); // View analytics data for your games.
                // scopes.Add("moderation:read"); // View your channel's moderation data including Moderators, Bans, Timeouts and Automod settings
                // scopes.Add("channel:read:redemptions"); // View your channel points custom reward redemptions
                // scopes.Add("channel:edit:commercial"); // Run commercials on a channel.
                // scopes.Add("channel:read:hype_train"); // View hype train data for a given channel.
                // scopes.Add("channel:read:stream_key"); // Read authorized user's stream key.
                // scopes.Add("channel:manage:extensions"); // Manage your channel's extension configuration, including activating extensions.
                // scopes.Add("user:edit:follows"); // Edit your follows.

                string scopeString = string.Join("+", scopes);

                var secret = TwitchConfiguration.ClientSecret;
                var clientId = TwitchConfiguration.ApiClientId;

                return new Uri($"https://id.twitch.tv/oauth2/authorize?response_type=token&client_id={clientId}&redirect_uri=http://www.discontinued.tv&scope={scopeString}&state={secret}&force_verify=true");
            } 
        }
    }
}