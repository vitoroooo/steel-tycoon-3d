using System;
using Steel.Core.Crew;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        private bool EventAvailable(double hour) => hour >= Rule("newcomerHours") &&
            (Crew.olderEvent < 0 || hour - Crew.olderEvent >= Rule("twoEventWindowHours"));
        private void RecordCrewEvent(double hour)
        { Crew.olderEvent = Crew.latestEvent; Crew.latestEvent = hour; }

        public string CrewStopReason(double hour, double toolCondition)
        {
            if (hour < Crew.investigateUntil) return "Lapak ditutup untuk investigasi K3.";
            if (hour < Crew.blockadeUntil) return "Akses jalan lapak sedang diblokade warga.";
            if (Crew.morale < Rule("strikeThreshold")) return "Regu mogok. Tunggu pemulihan atau naikkan gaji.";
            if (Crew.policy == ShiftPolicy.Hemat && !CrewSave.IsDay(hour)) return "Shift hemat hanya bekerja siang.";
            if (CrewOffline && toolCondition < Rule("unsafeCondition")) return "Alat tidak aman untuk kerja offline.";
            return null;
        }
        private bool CanInvestigate(double hour) => Yard.bandar && !CrewOffline &&
            hour >= Crew.investigateUntil && EventAvailable(hour);

        // Maintenance may send either a raw breakdown or an already rolled accident, never both.
        public void CrewEmergencyBreakdown(double hour)
        {
            if (CanInvestigate(hour) && Crew.Roll(Rule("breakdownChance"))) CrewConfirmedAccident(hour);
        }
        public void CrewConfirmedAccident(double hour) => InvestigateCrew(hour, 2);
        public void InvestigateCrew(double hour, int caste)
        {
            if (!CanInvestigate(hour)) return;
            RecordCrewEvent(hour);
            Crew.incidents++;
            Crew.investigateUntil = hour + Rule("investigationHours");
            if (Yard.payout > 0 && Yard.unloadHour > hour) Yard.unloadHour += Rule("investigationHours");
            Crew.morale = CrewSave.Meter(Crew.morale - Rule("incidentMorale"));
            Yard.reputation = (int)CrewSave.Meter(Yard.reputation - Rule("incidentReputation"));
            Crew.lastFine = Rule(caste >= 3 ? "fineK3" : "fineK2");
            ChargeCrew(Crew.lastFine);
        }

        private void CrewWorked(double hour, double condition, int caste)
        {
            Crew.workHour = (long)Math.Floor(hour);
            if (Crew.policy == ShiftPolicy.Lembur) Crew.overtimeWorked = true;
            if (!CanInvestigate(hour)) return;
            bool tired = Crew.Stamina(hour) < Rule("tiredThreshold");
            bool risky = condition < Rule("unsafeCondition") || Crew.policy == ShiftPolicy.Lembur && tired;
            double risk = risky ? Rule("unsafeChance") : tired ? Rule("tiredChance") : 0;
            if (Crew.Roll(risk * (tired ? 2 : 1))) InvestigateCrew(hour, caste);
        }
    }
}
