namespace BotBrown.Workers
{
    public interface ISoundProcessor
    {
        void Play(string filename, float volume);
    }
}