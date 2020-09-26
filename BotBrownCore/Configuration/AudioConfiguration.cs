namespace BotBrownCore.Configuration
{
    using BotBrown.Configuration;
    using NAudio.CoreAudioApi;
    using Newtonsoft.Json;
    using System.ComponentModel;

    public class AudioConfiguration : IChangeableConfiguration
    {
        private bool isInitialized;
        private readonly MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();

        public event PropertyChangedEventHandler PropertyChanged;

        public string TTSAudioDeviceName { get; set; }

        public string SoundCommandDeviceName { get; set; }

        [JsonIgnore]
        public MMDevice SelectedTTSDevice { get; private set; }

        [JsonIgnore]
        public MMDevice SelectedSoundCommandDevice { get; private set; }

        private bool isDirty;

        public AudioConfiguration()
        {
        }

        public void InitializeConfiguration()
        {
            if (isInitialized)
            {
                return;
            }

            foreach (var audioEndpoint in deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                if (audioEndpoint.FriendlyName == TTSAudioDeviceName)
                {
                    SelectedTTSDevice = audioEndpoint;
                }

                if (audioEndpoint.FriendlyName == SoundCommandDeviceName)
                {
                    SelectedSoundCommandDevice = audioEndpoint;
                }

                if (SelectedTTSDevice != null && SelectedSoundCommandDevice != null)
                {
                    break;
                }
            }

            if (SelectedTTSDevice == null)
            {
                SelectedTTSDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
                TTSAudioDeviceName = SelectedTTSDevice.FriendlyName;
                isDirty = true;
            }

            if (SelectedSoundCommandDevice == null)
            {
                SelectedSoundCommandDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
                SoundCommandDeviceName = SelectedSoundCommandDevice.FriendlyName;
                isDirty = true;
            }

            if (isDirty)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(TTSAudioDeviceName)));
                }

                isDirty = false;
            }

            isInitialized = true;
        }
    }
}
