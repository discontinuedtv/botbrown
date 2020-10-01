namespace BotBrown.Workers
{
    public class CustomSoundPathProvider : ISoundPathProvider
    {
        public CustomSoundPathProvider(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
