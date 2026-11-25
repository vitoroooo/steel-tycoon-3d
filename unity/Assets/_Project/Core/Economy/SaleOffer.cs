namespace Steel.Core.Economy
{
    public sealed class SaleOffer
    {
        public double Kilograms { get; }
        public double Revenue { get; }
        public double Profit { get; }
        public double QuoteTime { get; }
        public int InventoryRevision { get; }
        internal SaleOffer(double kilograms, double revenue, double time, int revision, double cost)
        { Kilograms = kilograms; Revenue = revenue; QuoteTime = time; InventoryRevision = revision; Profit = revenue - cost; }
    }
}
