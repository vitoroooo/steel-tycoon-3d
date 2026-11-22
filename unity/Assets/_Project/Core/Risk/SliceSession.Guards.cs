using System;
using Steel.Core.Risk;

namespace Steel.Core.State.Slice
{
    /// <summary>Guards on the gate grid. They are posted before trouble comes; a gang at the gate fights whoever is already there.</summary>
    public sealed partial class SliceSession
    {
        /// <summary>Hires a guard of a kind onto a post, or with kind 0 sends the one there home. Hiring is paid once.</summary>
        public void PostGuard(int cell, int kind)
        {
            RequireYardPost();
            if (cell < 0 || cell >= RiskRules.Cells || kind < 0 || kind > RiskRules.Kinds)
                throw new ArgumentOutOfRangeException(nameof(cell));
            if (Risk.demand) throw new InvalidOperationException("Preman sudah di gerbang. Regu jaga tidak bisa diubah.");
            if (Risk.guards[cell] == kind) return;
            if (kind > 0)
            {
                double cost = RiskTerms.GuardCost(kind);
                if (Cash < cost) throw new InvalidOperationException("Kas belum cukup untuk merekrut satpam.");
                Pay(cost); RecordOperatingProfit(-cost);
            }
            Risk.guards[cell] = kind;
        }

        public void SetAutoDefend(bool defend) { RequireYardPost(); Risk.defend = defend; }
    }
}
