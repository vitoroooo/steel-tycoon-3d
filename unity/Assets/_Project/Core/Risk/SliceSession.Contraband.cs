using System;
using Steel.Core.Crew;
using Steel.Core.Risk;
using Steel.Core.Trade.Spk;

namespace Steel.Core.State.Slice
{
    /// <summary>Contraband at the yard: a tempting margin, heat that builds with every load, and raids that follow the heat.</summary>
    public sealed partial class SliceSession
    {
        public RiskRules RiskTerms { get; private set; }
        public RiskSave Risk { get; private set; } = new RiskSave();

        public void OpenRisk(RiskRules rules) => RiskTerms = rules ?? throw new ArgumentNullException(nameof(rules));
        public void RestoreRisk(RiskSave saved) => Risk = RiskSave.Restore(saved);

        private void RequireYardPost()
        {
            if (!Yard.bandar) throw new InvalidOperationException("Perlu lapak Kasta 2.");
            if (Position.Distance(Haul.Yard) > Haul.DeskReach) throw new InvalidOperationException("Datangi pos muat di kavling Anda.");
        }

        /// <summary>Today's offer, unless there is none, it was answered, or the player has no yard to take it to.</summary>
        public ContrabandOffer? Contraband(double hour, double perKg)
        {
            long day = SpkRules.Day(hour);
            return Yard.bandar && Risk.offerDay != day ? ContrabandOffer.On(day, perKg, RiskTerms) : null;
        }

        /// <summary>Takes the load: heat rises, and either it sells through quietly or a raid seizes it. True when raided.</summary>
        public bool AcceptContraband(ContrabandOffer offer, double hour)
        {
            Answer(offer, hour);
            bool raided = Risk.Roll(RiskTerms.RaidChance(Risk.heat, Risk.Protected(hour)));
            Risk.heat += RiskTerms.Get("heatPerLoad");
            if (!raided) { CreditProceeds(offer.Gain, offer.Gain); Risk.note = offer.Name + " lolos. Sorotan naik."; return false; }
            double twice = Risk.exposed ? RiskTerms.Get("exposedFactor") : 1;
            ChargeOperatingExpense(offer.Fine * twice);
            Yard.reputation = (int)CrewSave.Meter(Yard.reputation - RiskTerms.Get("raidReputation") * twice);
            if (Caste >= 3) Risk.seizedUntil = hour + RiskTerms.Get("seizedHours");
            Risk.exposed = false;
            Risk.note = "RAZIA. Muatan disita, denda " + (offer.Fine * twice).ToString("N0") + ", reputasi turun.";
            return true;
        }

        public void RefuseContraband(ContrabandOffer offer, double hour) { Answer(offer, hour); Risk.note = "Tawaran ditolak."; }

        public void ReportContraband(ContrabandOffer offer, double hour)
        {
            Answer(offer, hour);
            Yard.reputation = (int)CrewSave.Meter(Yard.reputation + RiskTerms.Get("reportReputation"));
            Risk.note = "Tawaran dilaporkan. Reputasi +" + RiskTerms.Get("reportReputation") + ".";
        }

        private void Answer(ContrabandOffer offer, double hour)
        {
            RequireYardPost();
            if (offer.Day != SpkRules.Day(hour) || Risk.offerDay == offer.Day)
                throw new InvalidOperationException("Tawaran itu sudah lewat.");
            Risk.offerDay = offer.Day;
        }

        /// <summary>A paid protector halves raid chances for a game month, but each month may be exposed and double the next penalty.</summary>
        public void HireProtector(double hour)
        {
            RequireYardPost();
            if (Risk.Protected(hour)) throw new InvalidOperationException("Pelindung masih dibayar bulan ini.");
            double fee = RiskTerms.Get("protectorFee");
            if (Cash < fee) throw new InvalidOperationException("Kas belum cukup untuk pelindung.");
            Pay(fee); RecordOperatingProfit(-fee);
            Risk.protectorUntil = hour + RiskTerms.Get("monthHours");
            if (Risk.Roll(RiskTerms.Get("protectorExposed"))) { Risk.exposed = true; Risk.note = "Relasi pelindung terbongkar. Hukuman berikutnya berlipat."; }
        }
    }
}
