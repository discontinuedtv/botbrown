﻿namespace BotBrownCore.Configuration
{
    using BotBrown.Configuration;
    using NAudio.CoreAudioApi;
    using Newtonsoft.Json;
    using System.ComponentModel;

    public class AudioConfiguration : IConfiguration
    {
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
            foreach (var audioEndpoint in deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                if (audioEndpoint.DeviceFriendlyName == TTSAudioDeviceName)
                {
                    SelectedTTSDevice = audioEndpoint;
                }

                if (audioEndpoint.DeviceFriendlyName == SoundCommandDeviceName)
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
                TTSAudioDeviceName = SelectedTTSDevice.DeviceFriendlyName;
                isDirty = true;
            }

            if (SelectedSoundCommandDevice == null)
            {
                SelectedSoundCommandDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
                SoundCommandDeviceName = SelectedSoundCommandDevice.DeviceFriendlyName;
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
        }
    }
}
