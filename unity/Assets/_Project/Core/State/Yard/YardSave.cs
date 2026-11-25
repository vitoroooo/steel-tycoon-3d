using System;
using Steel.Core.State;

namespace Steel.Core.State.Saving
{
    [Serializable] public sealed class YardSave
    {
        public int reputation;
        public bool bandar, applied, leased;
        public double permitHour, leaseUntil, soldKg;
        public double[] bids = new double[4], stock = new double[4];
        internal static YardSave Restore(YardSave saved)
        {
            if (saved == null || !saved.bandar && (saved.bids == null || saved.bids.Length == 0) &&
                (saved.stock == null || saved.stock.Length == 0)) return new YardSave();
            if (saved.bids?.Length != 4 || saved.stock?.Length != 4 || saved.reputation < 0 ||
                saved.reputation > 100 || saved.bandar && (!saved.applied || !saved.leased))
                throw new FormatException("Data lapak rusak.");
            foreach (double x in saved.bids) NumberRules.NonNegative(x,"bid");
            foreach (double x in saved.stock) NumberRules.NonNegative(x,"stock");
            NumberRules.NonNegative(saved.permitHour,"permit"); NumberRules.NonNegative(saved.leaseUntil,"lease");
            NumberRules.NonNegative(saved.soldKg,"sold"); return saved;
        }
    }
}
