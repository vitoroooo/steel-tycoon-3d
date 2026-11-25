using System;

namespace Steel.Core.State.Saving
{
    [Serializable] public sealed class SatelliteSave
    {
        public int district;
        public double leaseUntil, bid, stock, cost;
        internal void Validate()
        {
            if (district < 1 || district > 2) throw new FormatException("Distrik lapak satelit tidak valid.");
            NumberRules.NonNegative(leaseUntil,"lease"); NumberRules.NonNegative(stock,"stock");
            NumberRules.NonNegative(bid,"bid");
            NumberRules.NonNegative(cost,"cost");
        }
    }
}
