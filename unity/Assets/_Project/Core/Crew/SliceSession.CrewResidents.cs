using System;
using Steel.Core.Crew;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public double CsrCost => Rule("csrCost");

        public void FundCrewCsr(double hour)
        {
            RequireCrewDesk();
            if (hour < Crew.csrReady) throw new InvalidOperationException("Program CSR masih berlangsung.");
            if (Crew.patience >= 100) throw new InvalidOperationException("Warga belum meminta bantuan.");
            if (Cash < CsrCost) throw new InvalidOperationException("Kas belum cukup untuk program CSR.");
            ChargeCrew(CsrCost);
            Crew.patience = CrewSave.Meter(Crew.patience + Rule("csrPatience"));
            Yard.reputation = (int)CrewSave.Meter(Yard.reputation + Rule("csrReputation"));
            Crew.csrReady = hour + Rule("csrCooldownHours");
        }
        private void CrewEnvironment(long hour, bool worked, int trucks, bool shredder, bool enclosed, bool rfid)
        {
            // A stuck inbound trailer occupies the public approach until the loader or tow clears it.
            if (Yard.stuck || Yard.rescued && Yard.rescueHour > hour - 1) trucks++;
            double penalty = Math.Max(0, trucks) * Rule("streetPenalty");
            if (worked && !CrewSave.IsDay(hour - 1)) penalty += Rule("nightNoise") * (enclosed ? .5 : 1);
            if (shredder && !enclosed) penalty += Rule("dustPenalty");
            double relief = penalty == 0 || rfid ? Rule("quietRecovery") : 0;
            Crew.patience = CrewSave.Meter(Crew.patience + relief - penalty);
            if (Crew.patience < Rule("blockadeThreshold") && Crew.blockadeUntil <= hour &&
                EventAvailable(hour))
            {
                RecordCrewEvent(hour);
                Crew.blockadeUntil = hour + Rule("blockadeHours");
            }
        }
    }
}
