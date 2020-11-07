namespace BotBrown.Configuration.Factories
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
