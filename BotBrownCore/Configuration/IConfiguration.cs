namespace BotBrown.Configuration
{
    public interface IConfiguration
    {
        string Filename { get; }

        bool IsValid();
    }
}