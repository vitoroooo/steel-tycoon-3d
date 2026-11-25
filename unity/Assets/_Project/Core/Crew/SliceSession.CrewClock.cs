using System;
using Steel.Core.Crew;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public double CrewCatchUp(double elapsed, double standardCap, Action<double> advance)
        {
            NumberRules.NonNegative(elapsed, nameof(elapsed));
            double applied = Math.Min(elapsed, CrewQueueLimit(standardCap));
            CrewOffline = true;
            try { advance(applied); }
            finally { CrewOffline = false; }
            return applied;
        }

        private double CrewUnloadHour(double hour, double duration)
        {
            duration /= CrewOutput(hour);
            if (Crew.policy != ShiftPolicy.Hemat) return hour + duration;
            double daylightLeft = 18 - hour % 24;
            double nightBreaks = Math.Ceiling(Math.Max(0, duration - daylightLeft) / 12);
            return hour + duration + 12 * nightBreaks;
        }
    }
}
