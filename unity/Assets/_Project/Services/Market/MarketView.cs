namespace Steel.Services
{
    public sealed class MarketView
    {
        public string Name, Rates, Direction;
        public double Distance, FreightPerKg;
        public bool AtCounter;
    }
    public sealed class LocalInvoice
    {
        public double Gross, Freight, Net, Margin, Kg;
        public bool Shipped;
    }
    public partial interface IGameService
    {
        int BuyerCount { get; }
        MarketView Buyer(int index);
        LocalInvoice InspectBid(int index);
        double SellLocal();
    }
}
