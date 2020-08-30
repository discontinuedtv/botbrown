using BotBrownCore.Configuration;

namespace BotBrownCore
{
    public interface IUsernameResolver
    {
        ChannelUser ResolveUsername(ChannelUser channelUser);
    }
}