using System;
using Steel.Core.State;

namespace Steel.Core.Equipment
{
    public enum WorkResult { Completed, Unavailable, WornOut, Breakdown, Accident }

    [Serializable] public sealed class MachineState
    {
        public double condition = 100, operatingHours, repairDue, riskUntil;
        public bool needsOriginal;
        public int randomState, breakdowns, accidents;

        internal void Validate()
        {
            NumberRules.NonNegative(condition, nameof(condition));
            NumberRules.NonNegative(operatingHours, nameof(operatingHours));
            NumberRules.NonNegative(repairDue, nameof(repairDue));
            NumberRules.NonNegative(riskUntil, nameof(riskUntil));
            if (condition > 100 || (repairDue > 0 || needsOriginal) && condition != 0 ||
                breakdowns < 0 || accidents < 0 || accidents > breakdowns)
                throw new FormatException("Data kondisi alat tidak valid.");
        }

        internal void Advance(double hour)
        {
            if (repairDue > 0 && hour >= repairDue)
            {
                condition = 100;
                repairDue = 0;
            }
            if (hour >= riskUntil) riskUntil = 0;
        }

        // Keep the next independent roll in the save, including the roll for a K3 incident.
        private bool Roll(double probability)
        {
            uint next = unchecked((uint)randomState * 1664525u + 1013904223u);
            randomState = unchecked((int)next);
            return next / 4294967296d < probability;
        }

        public WorkResult Work(MaintenanceRules rules, double hour, double duration, double loadRatio)
        {
            NumberRules.NonNegative(hour, nameof(hour));
            NumberRules.NonNegative(loadRatio, nameof(loadRatio));
            if (NumberRules.NonNegative(duration, nameof(duration)) == 0 || loadRatio > 1)
                throw new ArgumentOutOfRangeException();
            Advance(hour);
            if (condition <= 0) return WorkResult.Unavailable;
            bool heavy = loadRatio > rules.HeavyAbove;
            if (heavy && riskUntil > hour && Roll(rules.BreakdownChance))
            {
                condition = 0;
                riskUntil = 0;
                breakdowns++;
                if (!Roll(rules.AccidentChance)) return WorkResult.Breakdown;
                accidents++;
                return WorkResult.Accident;
            }
            double wear = heavy ? rules.HeavyWear : rules.NormalWear;
            double availableHours = condition / wear;
            operatingHours += Math.Min(duration, availableHours);
            condition = Math.Max(0, condition - wear * duration);
            return duration <= availableHours ? WorkResult.Completed : WorkResult.WornOut;
        }

        internal void StartRepair(RepairMethod method, double hour, MaintenanceRules rules)
        {
            repairDue = hour + rules.RepairHours(method);
            riskUntil = method == RepairMethod.Emergency ? repairDue + rules.RiskHours : 0;
            needsOriginal = false;
        }

        internal void Donate()
        {
            condition = 0;
            riskUntil = 0;
            needsOriginal = true;
        }
    }
}
