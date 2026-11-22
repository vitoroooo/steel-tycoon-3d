using System.Linq;
using Steel.Core.Scrap;
using Steel.Core.Trade.Spk;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        private SpkOffer[] SpkOffers => session.SpkTerms.Offers(SpkRules.Day(GameHours),
            grade => scrap.UnitRate((ScrapGrade)grade, 0, Prices.ScrapReference));

        public SpkOfferView[] SpkBoard => SpkOffers.Select(offer => new SpkOfferView
        {
            Grade = ScrapText.Grade((ScrapGrade)offer.Grade), Tonnes = offer.Kg / 1000, PerKg = offer.PerKg,
            Bond = offer.Value * session.SpkTerms.Bond, Days = (int)(offer.Hours / SpkRules.HoursPerDay),
            MinReputation = offer.MinReputation, Taken = session.Spk.taken.Contains(offer.Id)
        }).ToArray();

        public SpkView Spk
        {
            get
            {
                SpkSave spk = session.Spk;
                return new SpkView
                {
                    Active = spk.active, Note = spk.note, Grade = ScrapText.Grade((ScrapGrade)spk.grade),
                    Tonnes = spk.kg / 1000, DeliveredTonnes = spk.delivered / 1000, PerKg = spk.perKg, Bond = spk.bond,
                    HoursLeft = spk.deadline - GameHours, Reputation = session.Yard.reputation
                };
            }
        }

        public void TakeSpk(int slot) => session.TakeSpk(SpkOffers[slot], GameHours);
    }
}
