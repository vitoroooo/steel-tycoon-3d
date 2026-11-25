using Steel.Core.Scrap;

namespace Steel.Core.Economy.Market
{
    public sealed class LocalBid
    {
        public LocalBuyer Buyer { get; }
        public double Gross { get; }
        public double Freight { get; }
        public double Kg { get; }
        public double Net => Gross - Freight;
        public double Margin { get; }
        public bool Shipped { get; }
        internal readonly double Stamp;
        internal readonly int Revision;
        internal readonly Backpack Bag;
        internal LocalBid(LocalBuyer buyer, Backpack bag, double stamp, double kg, double gross,
            bool shipped, double freight, double acquisition)
        {
            Buyer = buyer; Bag = bag; Stamp = stamp; Revision = bag.Revision; Kg = kg;
            Gross = gross; Shipped = shipped; Freight = freight; Margin = Net - acquisition;
        }
    }
}
