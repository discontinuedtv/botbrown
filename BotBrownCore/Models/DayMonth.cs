namespace BotBrown.Configuration
{
    public class DayMonth
    {
        public int Day { get; set; }
        public int Month { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if (!(obj is DayMonth other))
                return false;

            return Day == other.Day && Month == other.Month;
        }

        public override int GetHashCode()
        {
            return Day.GetHashCode() + Month.GetHashCode();
        }
    }
}
