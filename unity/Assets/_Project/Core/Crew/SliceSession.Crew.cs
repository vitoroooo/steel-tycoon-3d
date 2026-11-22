using System;
using Steel.Core.Crew;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public CrewSave Crew { get; private set; } = new CrewSave();
        private ConfigValues crewRules;
        public bool CrewOffline { get; private set; }
        public Action<double> CrewExpense { private get; set; }

        public void InitCrew(ConfigValues rules) => crewRules = rules;
        public void RestoreCrew(CrewSave saved) => Crew = CrewSave.Restore(saved);
        private double Rule(string key) => crewRules.Get(key);
        public double CrewQueueLimit(double standard) =>
            standard * (Yard.bandar && Crew.senior ? Rule("seniorCapFactor") : 1) + LegacyQueueSeconds;

        private void RequireCrewDesk()
        {
            RequireNear(Rules.Home);
            if (!Yard.bandar) throw new InvalidOperationException("Mandor tersedia setelah lapak Kasta 2 dibuka.");
        }
        public void SetCrewPolicy(ShiftPolicy policy)
        {
            RequireCrewDesk();
            if (!Enum.IsDefined(typeof(ShiftPolicy), policy)) throw new ArgumentOutOfRangeException(nameof(policy));
            Crew.policy = policy;
        }
        public void PromoteForeman()
        {
            RequireCrewDesk();
            if (Crew.senior) throw new InvalidOperationException("Mandor sudah senior.");
            Pay(Rule("trainingCost")); Crew.senior = true;
        }
        public void SetCrewPremium(bool premium) { RequireCrewDesk(); Crew.premium = premium; }
        public double CrewWage => ((Crew.policy == ShiftPolicy.Hemat ? 1 : 2) * Rule("wageOperator") +
            Rule(Crew.senior ? "wageSenior" : "wageForeman")) * (Crew.premium ? Rule("premiumFactor") : 1);

        private void ChargeCrew(double amount)
        {
            if (CrewExpense != null) { CrewExpense(amount); return; }
            double paid = Math.Min(Cash, amount);
            Cash -= paid; Crew.unpaid += amount - paid;
        }
        private void CrewPayday()
        {
            double wage = CrewWage;
            bool late = Cash < wage;
            ChargeCrew(wage);
            double change = late ? -Rule("lateMorale") : Crew.premium ? Rule("premiumMorale") : 0;
            if (Crew.overtimeWorked) change -= Rule("overtimeMorale");
            Crew.overtimeWorked = false;
            Crew.morale = CrewSave.Meter(Crew.morale + change);
        }
        public double CrewOutput(double hour) => (Crew.policy == ShiftPolicy.Lembur ? Rule("overtimeFactor") : 1)
            * (Crew.Stamina(hour) < Rule("tiredThreshold") ? Rule("tiredFactor") : 1);

        // Preserve gains and losses from K3/CSR when the next ordinary sale grants reputation.
        public void CreditCrewSaleReputation(double kg)
        {
            double previous = Math.Floor(Yard.soldKg / 100);
            double bonus = FirstSaleMade ? 0 : 1;
            FirstSaleMade = true; Yard.soldKg += kg;
            Yard.reputation = (int)CrewSave.Meter(Yard.reputation + bonus + Math.Floor(Yard.soldKg / 100) - previous);
        }
    }
}
