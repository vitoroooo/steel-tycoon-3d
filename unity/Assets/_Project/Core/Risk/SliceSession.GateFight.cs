using System;
using System.Linq;
using Steel.Core.Crew;
using Steel.Core.Risk;

namespace Steel.Core.State.Slice
{
    /// <summary>Holding the gate (PRD appendix D.1). Won: no gang for two game months and +5 reputation.
    /// Lost: a month's security money plus the broken gate. Guards knocked out are back after a game day.</summary>
    public sealed partial class SliceSession
    {
        public BattleResult DefendGate(double hour) { RequireGang(); return Fight(hour); }

        private BattleResult Fight(double hour)
        {
            BattleResult result = GateBattle.Fight(Risk.OnDuty(hour), Risk.thugs, Caste, RiskTerms);
            Risk.demand = false;
            Risk.fainted = result.Down; Risk.faintUntil = hour + RiskTerms.Get("faintHours");
            DismissEscort();
            if (result.Held)
            {
                Risk.quietUntil = hour + RiskTerms.Get("winQuietHours");
                Yard.reputation = (int)CrewSave.Meter(Yard.reputation + RiskTerms.Get("winReputation"));
                Risk.note = "Gerbang bertahan " + result.Rounds + " detik. Preman mundur, reputasi +" +
                    RiskTerms.Get("winReputation") + ", " + result.GuardsDown + " satpam pingsan.";
                return result;
            }
            double loss = Risk.demandDue + RiskTerms.GateDamage(Caste);
            ChargeOperatingExpense(loss);
            Risk.quietUntil = hour + RiskTerms.Get("monthHours");
            Risk.note = "Gerbang jebol di gelombang " + (result.Waves + 1) + ". Uang keamanan dan perbaikan gerbang " +
                loss.ToString("N0") + ".";
            return result;
        }

        /// <summary>The hired escort came for this one gang only.</summary>
        private void DismissEscort()
        {
            for (int cell = 0; cell < RiskRules.Posts; cell++)
                if (Risk.guards[cell] == RiskRules.Escort) Risk.guards[cell] = 0;
        }
    }
}
