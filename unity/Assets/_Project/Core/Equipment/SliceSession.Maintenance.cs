using System;
using Steel.Core.Equipment;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public MaintenanceRules Maintenance { get; private set; }
        public event Action<double> MaintenanceAccident;
        public event Action<double> RepairPaid;
        public Func<bool> MaintenanceOffline { get; set; }
        public void ConfigureMaintenance(MaintenanceRules rules) =>
            Maintenance = rules ?? throw new ArgumentNullException(nameof(rules));

        private bool LoaderBusy(double hour) => Yard.payout > 0 && hour < Yard.unloadHour ||
            Yard.rescued && hour < Yard.rescueHour;

        public void RepairMachine(bool loader, RepairMethod method, double hour)
        {
            RequireNear(Rules.Home);
            if (!(loader ? HasLoader : HasScale)) throw new InvalidOperationException("Alat belum dimiliki.");
            var target = Yard.Machine(loader, hour);
            if (target.repairDue > hour || target.condition > 0 || loader && LoaderBusy(hour))
                throw new InvalidOperationException("Alat belum dapat diperbaiki.");
            if (target.needsOriginal && method != RepairMethod.Original)
                throw new InvalidOperationException("Alat donor memerlukan suku cadang asli.");
            var donor = Yard.Machine(!loader, hour);
            if (method == RepairMethod.Donor && (!(loader ? HasScale : HasLoader) || donor.condition <= 0 ||
                !loader && LoaderBusy(hour)))
                throw new InvalidOperationException("Alat donor harus siap dan menganggur.");
            double cost = Maintenance.RepairCost(loader, method);
            Pay(cost);
            if (method == RepairMethod.Donor) donor.Donate();
            target.StartRepair(method, hour, Maintenance);
            if (cost > 0) RepairPaid?.Invoke(cost);
        }

        private WorkResult UseMachine(bool loader, double hour, double duration, double loadRatio)
        {
            var machine = Yard.Machine(loader, hour);
            bool riskyLoad = machine.riskUntil > hour && loadRatio > Maintenance.HeavyAbove;
            if (riskyLoad && MaintenanceOffline?.Invoke() == true) return WorkResult.Unavailable;
            var result = machine.Work(Maintenance, hour, duration, loadRatio);
            if (result == WorkResult.Accident) MaintenanceAccident?.Invoke(hour);
            return result;
        }

        private void WorkLoader(double hour, double duration, double loadRatio)
        {
            switch (UseMachine(true, hour, duration, loadRatio))
            {
                case WorkResult.Completed: return;
                case WorkResult.Accident: throw new InvalidOperationException("Loader rusak. Terjadi kecelakaan K3.");
                case WorkResult.Breakdown: throw new InvalidOperationException("Perbaikan darurat gagal menahan beban.");
                case WorkResult.WornOut: throw new InvalidOperationException("Kondisi loader habis saat bekerja.");
                default: throw new InvalidOperationException("Loader rusak atau sedang diperbaiki.");
            }
        }
    }
}
