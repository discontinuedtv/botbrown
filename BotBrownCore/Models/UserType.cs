namespace BotBrown.Models
{
    public enum UserType
    {
        None = 0,
        Broadcaster = 1,
        Moderator = 2,
        Vip = 4,
        Subscriber = 8,
        Viewer = 16,
        Editor = Broadcaster | Moderator,
        AboveSubscriber = Editor | Vip,
        AllSubscribers = Editor | Vip | Subscriber,
        All = AllSubscribers | Viewer
    }

    public static class UserTypeExtensions
    {
        public static bool IsType(this UserType type, UserType other)
        {
            return (type & other) != UserType.None;
        }
    }
}
