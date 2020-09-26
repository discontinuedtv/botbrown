namespace BotBrown.Configuration.Factories
{
    using BotBrownCore.Configuration;

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
