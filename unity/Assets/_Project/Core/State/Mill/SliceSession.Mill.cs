using System;
using Steel.Core.State.Saving;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public MillSave Mill { get; private set; } = new MillSave();
        public int Caste => Mill.opened ? 3 : Yard.bandar ? 2 : 1;
        public void FileAmdal(ConfigValues rules, bool complete, double hour)
        {
            RequireNear(Rules.Factory);
            if (!complete) throw new InvalidOperationException("Lengkapi rencana pengendalian debu dan kebisingan.");
            if (!Yard.bandar || Mill.filed) throw new InvalidOperationException("AMDAL hanya sekali untuk bandar.");
            if (BusinessLevel < 28) throw new InvalidOperationException("AMDAL memerlukan level usaha 28.");
            Pay(rules.Positive("amdal")); Mill.filed = true; Mill.permitAt = hour + rules.Positive("permitHours");
        }
        public void OpenMill(ConfigValues rules, double hour)
        {
            RequireNear(Rules.Factory);
            if (Mill.opened || !Yard.bandar || !Mill.filed || hour < Mill.permitAt ||
                Yard.reputation < rules.Integer("rep") || Cash < rules.Positive("cash"))
                throw new InvalidOperationException("Perlu AMDAL aktif, reputasi 70 dan kas Rp3 miliar.");
            Pay(rules.Positive("package")); Mill.opened = true;
            Mill.plantValue = rules.Positive("package");
        }
        public void EquipMill(ConfigValues rules, int option)
        {
            RequireNear(Rules.Factory);
            if (!Mill.opened) throw new InvalidOperationException("Bangun pabrik dahulu.");
            switch (option)
            {
                case 0 when !Mill.shredder && BusinessLevel >= 35: BuyPlant(rules,"shredder"); Mill.shredder = true; break;
                case 1 when !Mill.rolling: BuyPlant(rules,"rolling"); Mill.rolling = true; break;
                case 2 when !Mill.bolts: BuyPlant(rules,"bolts"); Mill.bolts = true; break;
                case 3 when !Mill.rfid && BusinessLevel >= 35: BuyPlant(rules,"rfid"); Mill.rfid = true; break;
                default: throw new InvalidOperationException("Lini sudah terpasang atau tidak dikenal.");
            }
        }
        private void BuyPlant(ConfigValues rules,string key)
        { double price = rules.Positive(key); Pay(price); Mill.plantValue += price; }
        public void Melt(ConfigValues rules, int product, int grade, double hour)
        {
            RequireNear(Rules.Factory);
            if (!Mill.opened || product < 0 || product > 2 || grade < 0 || grade > 3 ||
                product == 1 && !Mill.rolling || product == 2 && !Mill.bolts)
                throw new InvalidOperationException("Lini produk belum tersedia.");
            Mill.Finish(hour);
            if (Mill.pendingKg > 0) throw new InvalidOperationException("Tungku masih beroperasi.");
            if (hour < Crew.investigateUntil) throw new InvalidOperationException("Tunggu investigasi K3 selesai.");
            double charge = rules.Positive("chargeKg");
            if (Yard.stock[grade] < charge) throw new InvalidOperationException("Stok satu grade belum cukup.");
            double output = Yard.BillableKg(grade,charge) *
                (1 - rules.Fraction(Mill.shredder ? "shreddedLoss" : "meltLoss"));
            double expense = output * rules.Positive("powerPerKg") + rules.Positive("operatorShift");
            Pay(expense);
            double part = charge/Yard.stock[grade], metalCost = accounts.stockCost[grade]*part;
            accounts.stockCost[grade] -= metalCost; accounts.knownKg[grade] *= 1-part;
            Mill.heatCost = expense + metalCost;
            Yard.InvoiceWeight(grade,charge,true); Yard.stock[grade] -= charge;
            Mill.product = product; Mill.pendingKg = output;
            Mill.completedAt = hour + charge / rules.Positive("dailyInputKg") * 24;
        }
        public void FinishHeat(double hour) => Mill.Finish(hour);
        public double SellSteel(ConfigValues rules, int product, double ibn)
        {
            RequireNear(Rules.Factory);
            if (!Mill.opened || product < 0 || product > 2 || Mill.goods[product] <= 0)
                throw new InvalidOperationException("Tidak ada produk siap dijual.");
            double kg = Mill.goods[product], factor = product == 1 ? rules.Positive("beamRate") :
                product == 2 ? rules.Positive("boltRate") : 1;
            double revenue = kg * ibn * factor;
            CollectSteelProceeds(product,kg,revenue); CreditSale(kg); return revenue;
        }
        private void CollectSteelProceeds(int product,double kg,double revenue)
        {
            double cost = Mill.costs[product]*(kg/Mill.goods[product]);
            Mill.goods[product] -= kg; Mill.costs[product] -= cost; CreditProceeds(revenue,revenue-cost);
        }
    }
}
