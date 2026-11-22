using System;
using Steel.Core.Trade.Street;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        /// <summary>The doors that sell scrap; null in a session built without the city's sellers.</summary>
        public StreetBlock Street { get; private set; }

        public void OpenStreet(StreetBlock street) => Street = street;

        private StreetSeller Door(string id)
        {
            var seller = Street.Find(id);
            if (Position.Distance(seller.Position) > Street.Rules.Reach)
                throw new InvalidOperationException("Dekati penjualnya terlebih dahulu.");
            return seller;
        }

        public void InspectSeller(string id) => Door(id).Inspect();
        public void WeighSeller(string id) => Door(id).Weigh();

        /// <summary>Names a price per kilogram; <paramref name="reference"/> is what the factory pays a collector now.</summary>
        public BargainReply Haggle(string id, double perKg, double reference) => Door(id).Offer(perKg / reference);

        /// <summary>Prices are rounded to whole rupiah per kilogram so the bill matches what the seller said aloud.</summary>
        public static double PerKg(StreetSeller seller, double reference) => Math.Round(seller.Ask * reference);

        public StreetBill Bill(string id, double reference)
        {
            var seller = Door(id);
            if (seller.Stock == null) throw new InvalidOperationException("Barangnya sudah habis. Datang lagi nanti.");
            double taken = Math.Min(seller.Stock.GrossKg, Bag.CapacityKg - Bag.CarriedKg);
            if (taken <= 0) throw new InvalidOperationException("Muatan penuh. Sortir dan jual dahulu.");
            double charged = seller.Charged(taken), perKg = PerKg(seller, reference);
            return new StreetBill(taken, charged, perKg, Math.Ceiling(charged * perKg));
        }

        /// <summary>Pays the bill and loads the goods. Returns what was paid.</summary>
        public double BuyAtDoor(string id, double reference)
        {
            var seller = Door(id);
            var bill = Bill(id, reference);
            if (!seller.Stock.KnownGrade.HasValue) throw new InvalidOperationException("Lihat dulu barangnya sebelum membayar.");
            if (Cash < bill.Total) throw new InvalidOperationException("Kas belum cukup untuk membayar barang ini.");
            Bag.Add(seller.Take(bill.TakenKg, bill.Total));
            Cash -= bill.Total;
            return bill.Total;
        }
    }
}
