using System;

namespace Steel.Core.State.Saving
{
    [Serializable] public sealed class TenderSave
    {
        public bool offered, awarded;
        public long round;
        public double bid, sent;
        public int repAtBid;
        public bool certified;
        public string last;

        public const int Interval = 120, Closing = 24, Deadline = 240, Tonnes = 300;
        public const double Target = Tonnes * 1000;
        public static long Current(double hour) => (long)Math.Floor(hour/Interval);
        public double Remaining => Target-sent;
        internal static TenderSave Restore(TenderSave saved)
        {
            if (saved == null) return new TenderSave();
            if (saved.round < 0 || saved.repAtBid < 0 || saved.repAtBid > 100 || saved.sent > Target ||
                saved.awarded && !saved.offered) throw new FormatException("Data tender tidak valid.");
            NumberRules.NonNegative(saved.bid,"bid"); NumberRules.NonNegative(saved.sent,"sent");
            return saved;
        }
        internal bool Score(double reference)
        {
            double competitor = reference*(.86+(round%4)*.025);
            double low = Math.Min(competitor,bid);
            double ours = 60*low/bid+repAtBid*.25+(certified ? 15 : 0);
            double rival = 60*low/competitor+14+(round%2==0 ? 0 : 15);
            return ours > rival;
        }
    }
}
