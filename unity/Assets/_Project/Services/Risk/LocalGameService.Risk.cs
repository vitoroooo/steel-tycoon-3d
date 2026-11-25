using System;
using System.Linq;
using Steel.Core.Risk;
using Steel.Core.Scrap;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        private ContrabandOffer? Contraband =>
            session.Contraband(GameHours, scrap.UnitRate(ScrapGrade.Hms1, 0, Prices.ScrapReference));

        public RiskView Risk
        {
            get
            {
                RiskSave risk = session.Risk; RiskRules rules = session.RiskTerms; ContrabandOffer? offer = Contraband;
                return new RiskView
                {
                    Open = session.Yard.bandar, Heat = risk.heat, Note = risk.note,
                    RaidChance = rules.RaidChance(risk.heat, risk.Protected(GameHours)),
                    QuietHours = Math.Max(0, risk.quietUntil - GameHours),
                    ProtectorHours = Math.Max(0, risk.protectorUntil - GameHours), ProtectorFee = rules.Get("protectorFee"),
                    Offer = offer.HasValue, OfferName = offer?.Name, OfferKg = offer?.Kg ?? 0,
                    OfferGain = offer?.Gain ?? 0, OfferFine = offer?.Fine ?? 0,
                    Demand = risk.demand, Haggled = risk.haggled, AutoDefend = risk.defend, Thugs = risk.demand ? risk.thugs : 0,
                    DemandDue = risk.demandDue, DemandHoursLeft = Math.Max(0, risk.demandUntil - GameHours),
                    Guards = (int[])risk.guards.Clone(),
                    Fainted = risk.fainted.Select(fell => fell && GameHours < risk.faintUntil).ToArray(),
                    GuardCost = Enumerable.Range(0, RiskRules.Kinds + 1).Select(kind => kind == 0 ? 0 : rules.GuardCost(kind)).ToArray()
                };
            }
        }

        public string AnswerContraband(int choice)
        {
            ContrabandOffer offer = Contraband ?? throw new InvalidOperationException("Tidak ada tawaran hari ini.");
            if (choice == 0) session.AcceptContraband(offer, GameHours);
            else if (choice == 1) session.RefuseContraband(offer, GameHours);
            else session.ReportContraband(offer, GameHours);
            return session.Risk.note;
        }

        public string AnswerGang(int choice)
        {
            if (choice == 0) session.PayGang(GameHours);
            else if (choice == 1) session.HaggleGang(GameHours);
            else session.DefendGate(GameHours);
            return session.Risk.note;
        }

        public void PostGuard(int cell, int kind) => session.PostGuard(cell, kind);
        public void ToggleAutoDefend() => session.SetAutoDefend(!session.Risk.defend);
        public void HireProtector() => session.HireProtector(GameHours);
    }
}
