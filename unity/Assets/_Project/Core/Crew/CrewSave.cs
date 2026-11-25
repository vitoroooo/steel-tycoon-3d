using System;
using Steel.Core.State;

namespace Steel.Core.Crew
{
    public enum ShiftPolicy { Hemat, Normal, Lembur }

    [Serializable] public sealed class CrewSave
    {
        public int version = 1;
        public ShiftPolicy policy = ShiftPolicy.Normal;
        public bool senior, premium;
        public double dayStamina = 100, nightStamina = 100, morale = 100, patience = 100;
        public double investigateUntil, blockadeUntil, csrReady, unpaid, lastFine;
        public double olderEvent = -1, latestEvent = -1;
        public long workHour = -1;
        public int riskSequence, incidents;
        public bool overtimeWorked;

        public static CrewSave Restore(CrewSave saved)
        {
            // Older careers have either a missing object or zero-initialized fields in JsonUtility.
            if (saved == null || saved.version == 0) return new CrewSave();
            if (saved.version != 1 || !Enum.IsDefined(typeof(ShiftPolicy), saved.policy) ||
                saved.workHour < -1 || saved.riskSequence < 0 || saved.incidents < 0)
                throw new FormatException("Data regu tidak valid.");
            foreach (double v in new[] { saved.dayStamina, saved.nightStamina, saved.morale, saved.patience })
                if (!(v >= 0 && v <= 100)) throw new FormatException("Meteran regu tidak valid.");
            foreach (double v in new[] { saved.investigateUntil, saved.blockadeUntil, saved.csrReady,
                     saved.unpaid, saved.lastFine })
                NumberRules.NonNegative(v, "crew");
            if (!(saved.olderEvent >= -1 && saved.latestEvent >= saved.olderEvent) ||
                double.IsInfinity(saved.latestEvent)) throw new FormatException("Riwayat K3 tidak valid.");
            return saved;
        }

        public static bool IsDay(double hour) => hour % 24 >= 6 && hour % 24 < 18;
        public double Stamina(double hour) => IsDay(hour) ? dayStamina : nightStamina;
        internal static double Meter(double value) => Math.Min(100, Math.Max(0, value));

        internal bool Roll(double chance)
        {
            if (chance <= 0) return false;
            riskSequence = riskSequence == int.MaxValue ? 1 : riskSequence + 1;
            uint hash = unchecked((uint)riskSequence * 747796405u + 2891336453u);
            hash = unchecked(((hash >> (int)((hash >> 28) + 4)) ^ hash) * 277803737u);
            return ((hash >> 22) ^ hash) / 4294967296d < chance;
        }
    }
}
