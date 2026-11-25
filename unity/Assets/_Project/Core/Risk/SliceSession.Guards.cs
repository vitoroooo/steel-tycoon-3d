using System;
using Steel.Core.Risk;

namespace Steel.Core.State.Slice
{
    /// <summary>Guards on the gate grid. They are posted before trouble comes; once a gang is at the gate only an escort team can be hired.</summary>
    public sealed partial class SliceSession
    {
        /// <summary>Hires a guard of a kind onto one of the two front rows, or with kind 0 sends the one there home. Hiring is paid once.</summary>
        public void PostGuard(int cell, int kind)
        {
            RequireYardPost();
            if (cell < 0 || cell >= RiskRules.Posts || kind < 0 || kind > RiskRules.Kinds)
                throw new InvalidOperationException("Satpam hanya berjaga di dua baris depan.");
            if (Risk.guards[cell] == kind) return;
            if (kind == RiskRules.Escort ? !Risk.demand || Risk.guards[cell] > 0 : Risk.demand)
                throw new InvalidOperationException(Risk.demand
                    ? "Preman sudah di gerbang. Hanya Tim Pengawal yang bisa disewa di pos kosong."
                    : "Tim Pengawal hanya disewa saat preman datang.");
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
