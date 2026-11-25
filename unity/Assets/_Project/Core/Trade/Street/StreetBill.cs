namespace Steel.Core.Trade.Street
{
    /// <summary>What buying at a door costs right now, before any money moves.</summary>
    public sealed class StreetBill
    {
        /// <summary>Kilograms that fit in what the buyer brought, and the kilograms charged for them.</summary>
        public double TakenKg { get; }
        public double ChargedKg { get; }
        public double PerKg { get; }
        public double Total { get; }

        internal StreetBill(double takenKg, double chargedKg, double perKg, double total)
        {
            TakenKg = takenKg; ChargedKg = chargedKg; PerKg = perKg; Total = total;
        }
    }
}
