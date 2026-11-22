using System;
using System.Linq;
using Steel.Core.Economy;
using Steel.Core.Economy.Market;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public LocalBid Bid(LocalBuyer buyer, AiQuote prices, double freightRate)
        {
            bool ship = Position.Distance(buyer.Position) > Rules.InteractRange;
            if (ship) RequireNear(Rules.Home);
            if (!Bag.IsSorted) throw new InvalidOperationException("Sortir muatan dahulu di lapak.");
            var lots = Bag.Items.Where(p => buyer.Accepts(p.KnownGrade.Value)).ToArray();
            if (lots.Length == 0) throw new InvalidOperationException("Tidak ada barang yang diterima pembeli ini.");
            double kg = lots.Sum(p => p.SaleWeightKg);
            double gross = lots.Sum(p => p.SaleWeightKg * buyer.Rate(pricing, prices, p.KnownGrade.Value, p.ReuseQuality));
            double freight = ship ? Math.Ceiling(lots.Sum(p => p.GrossKg) * freightRate *
                Rules.Home.Distance(buyer.Position)) : 0;
            if (gross < freight) throw new InvalidOperationException("Ongkir melebihi harga. Antar sendiri.");
            return new LocalBid(buyer, Bag, prices.RefreshedAtRealSeconds, kg, gross, ship, freight,
                lots.Sum(p => p.AcquisitionCost));
        }
        public double Settle(LocalBid old, AiQuote prices, double freightRate)
        {
            if (old == null) throw new InvalidOperationException("Periksa nota pembeli dahulu.");
            var next = Bid(old.Buyer, prices, freightRate);
            if (old.Bag != Bag || old.Stamp != next.Stamp || old.Revision != next.Revision || old.Shipped != next.Shipped)
                throw new InvalidOperationException("Nota berubah. Periksa harga, muatan dan ongkir lagi.");
            Cash += next.Net;
            if (next.Net > 0) CreditSale(next.Kg);
            Bag.RemoveWhere(p => old.Buyer.Accepts(p.KnownGrade.Value));
            return next.Net;
        }
    }
}
