using Microsoft.Data.Sqlite;
using Steel.Core.Time;
using Steel.Services;

namespace Steel.Career.Careers
{
    /// <summary>The market's own hour count: game hours since its city opened (server/market/clock.py).</summary>
    public sealed class MarketClock
    {
        private readonly double hourSeconds;
        public MarketClock(IConfigSource rules) { hourSeconds = new TimeConfig(rules.Read("Time")).RealSecondsPerGameHour; }

        public long Hour(SqliteConnection db, long nowMs)
        {
            using SqliteCommand ask = db.CreateCommand();
            ask.CommandText = "SELECT opened_at FROM city";
            object opened = ask.ExecuteScalar();
            return opened == null ? 0 : (long)System.Math.Max(0, (nowMs / 1000 - (long)opened) / hourSeconds);
        }
    }
}
