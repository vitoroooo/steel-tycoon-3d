using System;
using System.Linq;

namespace Steel.Core.State.Saving
{
    [Serializable] public sealed class AssetUse
    {
        public string id;
        public int uses;
    }

    [Serializable] public sealed class FinanceSave
    {
        public int version = 1, level = 1;
        public double seconds, interestDue, debt;
        public double lifetimeProfit, quarterProfit, lastTax;
        public double[] stockCost = new double[4], knownKg = new double[4];
        public double shipmentMargin;
        public bool scaleSeized, loaderSeized;
        public AssetUse[] activity = Array.Empty<AssetUse>();
        public string notice;

        internal FinanceSave Copy()
        {
            var copy = (FinanceSave)MemberwiseClone();
            copy.stockCost = (double[])stockCost.Clone();
            copy.knownKg = (double[])knownKg.Clone();
            copy.activity = (activity ?? Array.Empty<AssetUse>()).Select(a =>
                new AssetUse { id = a.id, uses = a.uses }).ToArray();
            return copy;
        }
    }
}
