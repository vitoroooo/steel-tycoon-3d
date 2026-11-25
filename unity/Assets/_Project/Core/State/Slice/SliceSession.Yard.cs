using System;
using System.Linq;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        private void CreditSale(double kg)
        {
            FirstSaleMade = true; Yard.soldKg += kg;
            Yard.reputation = (int)Math.Min(100, 1 + Math.Floor(Yard.soldKg / 100));
        }
        private void Pay(double price)
        {
            if (Cash < price) throw new InvalidOperationException("Kas belum cukup.");
            Cash -= NumberRules.NonNegative(price, nameof(price));
        }
        public void ApplyPermit(ConfigValues v, double hour, bool complete)
        {
            RequireNear(Rules.Home);
            if (Yard.applied) throw new InvalidOperationException("Permohonan sudah terdaftar.");
            Pay(v.Positive("permit")); Yard.applied = true;
            Yard.permitHour = hour + v.Positive("permitHours") * (complete ? 1 : 2);
        }
        public void Lease(ConfigValues v, double hour)
        {
            RequireNear(Rules.Home);
            if (Yard.leaseUntil > hour) throw new InvalidOperationException("Sewa masih aktif.");
            Pay(v.Positive("rent")); Yard.leased = true; Yard.leaseUntil = hour + v.Positive("leaseHours");
        }
        public void OpenYard(ConfigValues v, double hour)
        {
            RequireNear(Rules.Home);
            if (Yard.bandar || !Yard.applied || hour < Yard.permitHour || hour >= Yard.leaseUntil ||
                Yard.reputation < v.Integer("minimumReputation") || Cash < v.Positive("minimumCash"))
                throw new InvalidOperationException("Perlu izin aktif, sewa aktif, reputasi 30 dan kas Rp250 juta.");
            Pay(v.Positive("scale") + v.Positive("loader")); Yard.bandar = true;
        }
        public void SetBid(int grade, double price, double hour)
        {
            RequireNear(Rules.Home);
            if (!Yard.bandar || hour >= Yard.leaseUntil) throw new InvalidOperationException("Lapak belum aktif.");
            Yard.bids[grade] = NumberRules.NonNegative(price, nameof(price));
        }
        public void Receive(int grade, double floorRate, double kg, double capacity, long hour)
        {
            if (!Yard.bandar || hour >= Yard.leaseUntil || Yard.bids[grade] < floorRate) return;
            double price = Yard.bids[grade] * kg;
            if (Cash < price || Yard.stock.Sum() + kg > capacity) return;
            Pay(price); Yard.stock[grade] += kg;
            if (grade == 0) Yard.exposed.Add(new Saving.SteelBatch { kg = kg });
        }
        public double ShipGrade(int grade, ConfigValues v, double price, double hour, int soil)
        {
            RequireNear(Rules.Home);
            double kg = v.Positive("minShipmentKg"), freight = v.Positive("shipFee");
            if (!Yard.bandar || Yard.stock[grade] < kg) throw new InvalidOperationException("Perlu 15 ton satu grade.");
            if (Yard.payout > 0) throw new InvalidOperationException("Trailer sebelumnya masih bongkar muat.");
            double net = Yard.InvoiceWeight(grade,kg,false)*price - freight;
            if (net <= 0) throw new InvalidOperationException("Nilai kiriman tidak menutup ongkos.");
            AdmitTrailer(hour,soil); Yard.stock[grade] -= kg; Yard.InvoiceWeight(grade,kg,true);
            Yard.payout = net; Yard.dispatchedKg = kg;
            Yard.unloadHour = hour + kg/10000 * (soil != 0 && !Yard.paved ? 1.3 : 1);
            return net;
        }
    }
}
