using System;
using System.Globalization;
using Steel.Core.Economy.Market;
using Steel.Core.Scrap;
using Steel.Core.State;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        private LocalBuyer[] buyers;
        private double freight;
        private LocalBid invoice;
        private void CreateBuyers(IConfigSource source)
        {
            var config = new ConfigValues(source.Read("LocalMarket"));
            buyers = LocalBuyer.Read(config); freight = config.Positive("freightPerKgMetre");
        }
        public int BuyerCount => buyers.Length;
        public MarketView Buyer(int index)
        {
            var b = buyers[index]; var pos = session.Position;
            double distance = pos.Distance(b.Position);
            string Rate(ScrapGrade g, int q = 0) => b.Rate(scrap, Prices, g, q)
                .ToString("N0", CultureInfo.GetCultureInfo("id-ID"));
            var directions = new[] { "utara", "timur laut", "timur", "tenggara", "selatan", "barat daya", "barat", "barat laut" };
            int sector = (int)Math.Round(Math.Atan2(b.Position.X - pos.X, b.Position.Z - pos.Z) * 4 / Math.PI);
            return new MarketView {
                Name = b.Name, Distance = distance, Direction = directions[(sector + 8) % 8],
                AtCounter = distance <= session.Rules.InteractRange,
                FreightPerKg = session.Rules.Home.Distance(b.Position) * freight,
                Rates = b.ReuseOnly ? "Layak pakai   " + Rate(ScrapGrade.Reuse) + "–" + Rate(ScrapGrade.Reuse, 1) :
                    "HMS 1   " + Rate(ScrapGrade.Hms1) + "     HMS 2   " + Rate(ScrapGrade.Hms2) +
                    "\nBesi cor   " + Rate(ScrapGrade.CastIron) + "     Campur   " + Rate(ScrapGrade.Mixed)
            };
        }
        private InteractionView NearbyBuyer()
        {
            for (int i = 0; i < buyers.Length; i++)
                if (session.Position.Distance(buyers[i].Position) <= session.Rules.InteractRange)
                    return new InteractionView { Id = "buyer", Title = buyers[i].Name, Detail = "Cek harga & nota penjualan" };
            return null;
        }
        public LocalInvoice InspectBid(int index)
        {
            invoice = null; pending = null;
            invoice = session.Bid(buyers[index], Prices, freight);
            return new LocalInvoice { Gross = invoice.Gross, Freight = invoice.Freight, Net = invoice.Net,
                Margin = invoice.Margin, Kg = invoice.Kg, Shipped = invoice.Shipped };
        }
        public double SellLocal()
        {
            var old = invoice; invoice = null;
            return session.Settle(old, Prices, freight);
        }
    }
}
