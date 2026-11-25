using System;
using Steel.Core.State;

namespace Steel.Core.Time
{
    public sealed class GameClock
    {
        private readonly TimeConfig config;
        public double RealSeconds { get; private set; }
        public double GameHours => RealSeconds / config.RealSecondsPerGameHour;
        public event Action<long> HourTick;

        public GameClock(TimeConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void Advance(double realSeconds)
        {
            NumberRules.NonNegative(realSeconds, nameof(realSeconds));
            double target = RealSeconds + realSeconds;
            double lastHour = Math.Floor(target / config.RealSecondsPerGameHour);
            if (double.IsInfinity(target) || lastHour >= long.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(realSeconds));
            long first = (long)Math.Floor(GameHours) + 1;
            for (long hour = first; hour <= (long)lastHour; hour++)
            {
                RealSeconds = hour * config.RealSecondsPerGameHour;
                HourTick?.Invoke(hour);
            }
            RealSeconds = target;
        }

        public double CatchUp(double elapsedRealSeconds)
        {
            NumberRules.NonNegative(elapsedRealSeconds, nameof(elapsedRealSeconds));
            double applied = Math.Min(elapsedRealSeconds, config.OfflineCapRealSeconds);
            Advance(applied);
            return applied;
        }
    }
}
