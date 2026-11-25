using System;
using System.Linq;
using Steel.Core.Equipment;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        private void CreditSale(double kg)
            => CreditCrewSaleReputation(kg);
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
            RecordOperatingProfit(-v.Positive("rent"));
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
            if (!HasScale || hour >= Yard.leaseUntil || Yard.bids[grade] < floorRate) return;
            double price = Yard.bids[grade] * kg;
            if (Cash < price || Yard.stock.Sum() + kg > capacity) return;
            if (kg > Maintenance.ScaleCapacity || UseMachine(false, hour, Maintenance.ScaleHours,
                kg / Maintenance.ScaleCapacity) != WorkResult.Completed) return;
            Pay(price); Yard.stock[grade] += kg;
            RecordStockPurchase(grade,kg,price);
            if (grade == 0) Yard.exposed.Add(new Saving.SteelBatch { kg = kg });
        }
        public double ShipGrade(int grade, ConfigValues v, double price, double hour, int soil)
        {
            RequireNear(Rules.Home);
            double kg = v.Positive("minShipmentKg"), freight = v.Positive("shipFee");
            if (!Yard.bandar || Yard.stock[grade] < kg) throw new InvalidOperationException("Perlu 15 ton satu grade.");
            if (Yard.payout > 0) throw new InvalidOperationException("Trailer sebelumnya masih bongkar muat.");
            if (!HasLoader) throw new InvalidOperationException("Ganti loader yang disita di halaman Keuangan.");
            double net = Yard.InvoiceWeight(grade,kg,false)*price - freight;
            if (net <= 0) throw new InvalidOperationException("Nilai kiriman tidak menutup ongkos.");
            if (kg > Maintenance.TrailerCapacity) throw new InvalidOperationException("Muatan melebihi kapasitas trailer.");
            if (Yard.Machine(true, hour).condition <= 0)
                throw new InvalidOperationException("Perbaiki loader di halaman Perawatan alat.");
            double duration = kg / Maintenance.LoaderRate * (soil != 0 && !Yard.paved ? 1.3 : 1);
            AdmitTrailer(hour, soil);
            WorkLoader(hour, duration, kg / Maintenance.TrailerCapacity);
            BookShipmentMargin(grade,kg,Yard.stock[grade],net);
            Yard.stock[grade] -= kg; Yard.InvoiceWeight(grade,kg,true);
            Yard.payout = net; Yard.dispatchedKg = kg;
            Yard.unloadHour = hour + duration;
            return net;
        }
    }
}
