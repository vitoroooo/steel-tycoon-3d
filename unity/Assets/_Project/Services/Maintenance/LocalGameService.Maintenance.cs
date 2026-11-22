using System;
using Steel.Core.Equipment;

namespace Steel.Services
{
    public struct MaintenanceView
    {
        public bool Owned, NeedsOriginal;
        public double Condition, OperatingHours, RepairRemaining, RiskRemaining;
        public int Breakdowns, Accidents;
        public double OriginalPrice, EmergencyPrice, OriginalHours, EmergencyHours, DonorHours;
        public double NormalWear, HeavyWear, HeavyAbove, RiskHours, BreakdownChance;
    }

    public partial interface IGameService
    {
        // Repair choices in ledger order: original parts, donor, emergency.
        MaintenanceView MaintenanceInfo(bool loader);
        void RepairMachine(bool loader, int choice);
    }

    public sealed partial class LocalGameService
    {
        private void InitMaintenance(IConfigSource source) =>
            session.ConfigureMaintenance(new MaintenanceRules(source.Read("Maintenance"), business));

        public MaintenanceView MaintenanceInfo(bool loader)
        {
            if (!session.Yard.bandar) return default;
            var rules = session.Maintenance;
            var machine = session.Yard.Machine(loader, GameHours);
            return new MaintenanceView
            {
                Owned = true, Condition = machine.condition, OperatingHours = machine.operatingHours,
                RepairRemaining = Math.Max(0, machine.repairDue - GameHours),
                RiskRemaining = Math.Max(0, machine.riskUntil - Math.Max(machine.repairDue, GameHours)),
                NeedsOriginal = machine.needsOriginal, Breakdowns = machine.breakdowns, Accidents = machine.accidents,
                OriginalPrice = rules.RepairCost(loader, RepairMethod.Original),
                EmergencyPrice = rules.RepairCost(loader, RepairMethod.Emergency),
                OriginalHours = rules.RepairHours(RepairMethod.Original),
                EmergencyHours = rules.RepairHours(RepairMethod.Emergency), DonorHours = rules.RepairHours(RepairMethod.Donor),
                NormalWear = rules.NormalWear, HeavyWear = rules.HeavyWear, HeavyAbove = rules.HeavyAbove,
                RiskHours = rules.RiskHours, BreakdownChance = rules.BreakdownChance
            };
        }

        public void RepairMachine(bool loader, int choice) =>
            session.RepairMachine(loader, (RepairMethod)choice, GameHours);
    }
}
