using System;
using Steel.Core.Crew;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public void CrewReceive(int grade, double floorRate, double kg, double capacity,
            long hour, double scaleCondition, int caste = 2)
        {
            if (!Yard.bandar || CrewStopReason(hour, scaleCondition) != null) return;
            double before = Yard.stock[grade];
            Receive(grade, floorRate, kg * CrewOutput(hour), capacity, hour);
            if (Yard.stock[grade] > before) CrewWorked(hour, scaleCondition, caste);
        }

        public double CrewShipGrade(int grade, ConfigValues config, double price, double hour, int soil,
            double loaderCondition, int caste = 2)
        {
            string stop = CrewStopReason(hour, loaderCondition);
            if (stop != null) throw new InvalidOperationException(stop);
            double net = ShipGrade(grade, config, price, hour, soil);
            Yard.unloadHour = CrewUnloadHour(hour, Yard.unloadHour - hour);
            CrewWorked(hour, loaderCondition, caste);
            return net;
        }

        public void CrewRescue(bool tow, double hour, double cost, double loaderCondition, int caste = 2)
        {
            if (!tow)
            {
                string stop = CrewStopReason(hour, loaderCondition);
                if (stop != null) throw new InvalidOperationException(stop);
            }
            Rescue(tow, hour, cost);
            if (!tow) CrewWorked(hour, loaderCondition, caste);
        }

        public void CrewHour(long hour, int streetTrucks = 0, bool shredderRunning = false,
            bool enclosed = false, bool rfid = false)
        {
            if (!Yard.bandar || hour <= 0) return;
            long start = hour - 1;
            bool loading = Yard.payout > 0 && Yard.unloadHour > start && Crew.investigateUntil <= start &&
                (Crew.policy != ShiftPolicy.Hemat || CrewSave.IsDay(start));
            bool worked = Crew.workHour == start || loading || shredderRunning;
            bool day = CrewSave.IsDay(start);
            // Long shifts overlap for the final two hours before a shift change.
            bool overlap = Crew.policy == ShiftPolicy.Lembur &&
                (start % 24 >= 4 && start % 24 < 6 || start % 24 >= 16 && start % 24 < 18);
            double recovery = Rule("restRecovery"), fatigue = Rule("workFatigue");
            Crew.dayStamina = CrewSave.Meter(Crew.dayStamina + (worked && (day || overlap) ? -fatigue : recovery));
            Crew.nightStamina = CrewSave.Meter(Crew.nightStamina + (worked && (!day || overlap) ? -fatigue : recovery));
            if (worked && Crew.policy == ShiftPolicy.Lembur) Crew.overtimeWorked = true;
            if (!worked && Crew.morale < Rule("strikeThreshold"))
                Crew.morale = CrewSave.Meter(Crew.morale + Rule("restMorale"));
            if (Crew.unpaid > 0)
            {
                double paid = Math.Min(Cash, Crew.unpaid);
                Cash -= paid; Crew.unpaid -= paid;
            }
            CrewEnvironment(hour, worked, streetTrucks, shredderRunning, enclosed, rfid);
            if (hour % 24 == 0) CrewPayday();
        }
    }
}
