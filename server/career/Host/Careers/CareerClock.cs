using System;

namespace Steel.Career.Careers
{
    /// <summary>How far a career's game clock may stand from the server's own clock. A phone suggests the time of each
    /// call so its guess and the server's result agree, but it can neither hurry time nor hold it back.</summary>
    public sealed class CareerClock
    {
        /// <summary>Seconds a phone's clock may run behind or ahead of the server's before the server's clock wins.</summary>
        public const double Behind = 10, Ahead = 2;
        /// <summary>A call run this far from the time the phone asked for means the phone's clock is wrong.</summary>
        public const double Drift = 0.5;
        /// <summary>Without a call for this long the player is away, and the time is counted as offline time (PRD 12).</summary>
        public const double AwaySeconds = 90;

        private double gameAnchor;
        private long wallAnchor;
        public long SeenMs { get; private set; }

        public CareerClock(double gameSeconds, long nowMs) { Anchor(gameSeconds, nowMs); }

        public void Anchor(double gameSeconds, long nowMs) { gameAnchor = gameSeconds; wallAnchor = SeenMs = nowMs; }

        public double AwayFor(long nowMs) => Math.Max(0, (nowMs - SeenMs) / 1000d);

        /// <summary>The game time a call asking for <paramref name="asked"/> is run at.</summary>
        public double Granted(double asked, double current, long nowMs)
        {
            double server = gameAnchor + (nowMs - wallAnchor) / 1000d;
            if (double.IsNaN(asked)) asked = server;
            return Math.Max(current, Math.Min(server + Ahead, Math.Max(server - Behind, asked)));
        }

        public void Seen(long nowMs) => SeenMs = Math.Max(SeenMs, nowMs);
    }
}
