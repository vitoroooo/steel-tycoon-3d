using System;
using System.Linq;
using Steel.Core.Risk;
using Steel.Core.Trade.Spk;

namespace Steel.Core.State.Slice
{
    /// <summary>A gang at the yard's gate (PRD 6.3): pay 2% of the month's sales or talk it down to 1%. Holding the gate is in GateFight.</summary>
    public sealed partial class SliceSession
    {
        /// <summary>Once every game hour: cools the heat, closes the month, lets a gang arrive, and answers one nobody answered.</summary>
        public void AdvanceRisk(double hour)
        {
            if (!Yard.bandar) return;
            long day = SpkRules.Day(hour), month = (long)Math.Floor(hour / RiskTerms.Get("monthHours"));
            if (day > Risk.cooledDay)
            {
                Risk.heat = Math.Max(0, Risk.heat - RiskTerms.Get("heatDecayPerDay") * (day - Risk.cooledDay));
                Risk.cooledDay = day;
            }
            if (month > Risk.month) { Risk.lastMonth = month == Risk.month + 1 ? Risk.turnover : 0; Risk.turnover = 0; Risk.month = month; }
            if (Risk.demand)
            {
                if (hour < Risk.demandUntil) return;
                if (Risk.defend) Fight(hour); else Settle(RiskTerms.Get("payShare"), hour, "Preman dibayar otomatis");
            }
            else if (day > Risk.rolledDay)
            {
                Risk.rolledDay = day;
                if (hour < Risk.quietUntil || !EventAvailable(hour) ||
                    !Risk.Roll(RiskTerms.ExtortChance(Cash, Risk.OnDuty(hour).Count(kind => kind > 0)))) return;
                RecordCrewEvent(hour);
                Risk.demand = true; Risk.haggled = false; Risk.thugs = RiskTerms.Thugs(Cash);
                Risk.demandDue = Math.Max(RiskTerms.Get("minDemand"), Math.Max(Risk.turnover, Risk.lastMonth) * RiskTerms.Get("payShare"));
                Risk.demandUntil = hour + RiskTerms.Get("answerHours");
                Risk.note = Risk.thugs + " preman menunggu di gerbang lapak.";
            }
        }

        private void RequireGang()
        {
            RequireYardPost();
            if (!Risk.demand) throw new InvalidOperationException("Tidak ada preman di gerbang.");
        }

        public void PayGang(double hour)
        {
            RequireGang();
            if (Cash < Risk.demandDue) throw new InvalidOperationException("Kas belum cukup untuk uang keamanan.");
            Settle(RiskTerms.Get("payShare"), hour, "Uang keamanan dibayar");
        }

        /// <summary>One try: reputation out of a hundred is the chance they take half. True when they do.</summary>
        public bool HaggleGang(double hour)
        {
            RequireGang();
            if (Risk.haggled) throw new InvalidOperationException("Mereka tidak mau bicara lagi.");
            if (Cash < Risk.demandDue / 2) throw new InvalidOperationException("Kas belum cukup untuk menawar.");
            Risk.haggled = true;
            if (!Risk.Roll(Yard.reputation / 100d)) { Risk.note = "Negosiasi gagal. Bayar penuh atau bertahan."; return false; }
            Settle(RiskTerms.Get("negotiateShare"), hour, "Negosiasi berhasil, setengah harga");
            return true;
        }

        private void Settle(double share, double hour, string how)
        {
            double due = Risk.demandDue * share / RiskTerms.Get("payShare");
            ChargeOperatingExpense(due);
            DismissEscort();
            Risk.demand = false; Risk.quietUntil = hour + RiskTerms.Get("monthHours");
            Risk.note = how + ": " + due.ToString("N0") + ". Tenang satu bulan game.";
        }
    }
}
