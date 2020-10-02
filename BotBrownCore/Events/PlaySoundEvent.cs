namespace BotBrown.Events
{
    public class PlaySoundEvent : Event
    {
        public PlaySoundEvent(string filename, float volume)
        {
            Filename = filename;
            Volume = volume;
        }

        public string Filename { get; }

        public float Volume { get; }
    }
}
