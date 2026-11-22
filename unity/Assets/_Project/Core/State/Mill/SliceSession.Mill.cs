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
            Pay(rules.Positive("amdal")); Mill.filed = true; Mill.permitAt = hour + rules.Positive("permitHours");
        }
        public void OpenMill(ConfigValues rules, double hour)
        {
            RequireNear(Rules.Factory);
            if (Mill.opened || !Yard.bandar || !Mill.filed || hour < Mill.permitAt ||
                Yard.reputation < rules.Integer("rep") || Cash < rules.Positive("cash"))
                throw new InvalidOperationException("Perlu AMDAL aktif, reputasi 70 dan kas Rp3 miliar.");
            Pay(rules.Positive("package")); Mill.opened = true;
        }
        public void EquipMill(ConfigValues rules, int option)
        {
            RequireNear(Rules.Factory);
            if (!Mill.opened) throw new InvalidOperationException("Bangun pabrik dahulu.");
            switch (option)
            {
                case 0 when !Mill.shredder: Pay(rules.Positive("shredder")); Mill.shredder = true; break;
                case 1 when !Mill.rolling: Pay(rules.Positive("rolling")); Mill.rolling = true; break;
                case 2 when !Mill.bolts: Pay(rules.Positive("bolts")); Mill.bolts = true; break;
                default: throw new InvalidOperationException("Lini sudah terpasang atau tidak dikenal.");
            }
        }
        public void Melt(ConfigValues rules, int product, int grade, double hour)
        {
            RequireNear(Rules.Factory);
            if (!Mill.opened || product < 0 || product > 2 || grade < 0 || grade > 3 ||
                product == 1 && !Mill.rolling || product == 2 && !Mill.bolts)
                throw new InvalidOperationException("Lini produk belum tersedia.");
            Mill.Finish(hour);
            if (Mill.pendingKg > 0) throw new InvalidOperationException("Tungku masih beroperasi.");
            double charge = rules.Positive("chargeKg");
            if (Yard.stock[grade] < charge) throw new InvalidOperationException("Stok satu grade belum cukup.");
            double output = Yard.BillableKg(grade,charge) *
                (1 - rules.Fraction(Mill.shredder ? "shreddedLoss" : "meltLoss"));
            Pay(output * rules.Positive("powerPerKg") + rules.Positive("operatorShift"));
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
            Cash += revenue; Mill.goods[product] = 0; CreditSale(kg); return revenue;
        }
    }
}
