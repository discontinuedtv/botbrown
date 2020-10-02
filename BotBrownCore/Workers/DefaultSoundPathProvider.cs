using System.IO;

namespace BotBrown.Workers
{
    public class DefaultSoundPathProvider : ISoundPathProvider
    {
        public string Path
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
        }
    }
}
