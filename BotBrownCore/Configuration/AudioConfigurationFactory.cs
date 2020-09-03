using BotBrownCore.Configuration;

namespace BotBrown.Configuration
{
    public class AudioConfigurationFactory : IConfigurationFileFactory<AudioConfiguration>
    {
        public AudioConfiguration CreateDefaultConfiguration()
        {
            return new AudioConfiguration
            {
                TTSAudioDeviceName = "",
                SoundCommandDeviceName = ""
            };
        }
    }
}
