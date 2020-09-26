namespace BotBrown.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    [ConfigurationFile(ConfigurationFileConstants.DeathCounter)]
    public class DeathCounterConfiguration : IChangeableConfiguration
    {
        public Dictionary<string, int> DeathsPerGame { get; set; } = new Dictionary<string, int>();

        public event PropertyChangedEventHandler PropertyChanged;

        public int GetDeath(string game)
        {
            if (DeathsPerGame.TryGetValue(game, out var counter))
            {
                return counter;
            }

            return 0;
        }

        public int DecreaseDeath(string game)
        {
            int newCount = 0;
            if (DeathsPerGame.ContainsKey(game))
            {
                newCount = DeathsPerGame[game];

                if (newCount == 1)
                {
                    DeathsPerGame.Remove(game);
                }
                else
                {
                    newCount--;
                    DeathsPerGame[game] = newCount;
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeathsPerGame)));
            return newCount;
        }

        public int IncreaseDeath(string game)
        {
            int newCount = 1;
            if (DeathsPerGame.ContainsKey(game))
            {
                newCount = DeathsPerGame[game];
                newCount++;
                DeathsPerGame[game] = newCount;
            }
            else
            {
                DeathsPerGame[game] = 1;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeathsPerGame)));
            return newCount;
        }

        public bool IsValid()
        {
            return true;
        }
    }
}