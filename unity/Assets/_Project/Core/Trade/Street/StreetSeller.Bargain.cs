using System;
using Steel.Core.State.Saving;

namespace Steel.Core.Trade.Street
{
    public sealed partial class StreetSeller
    {
        private const double Slack = 1e-9;   // a saved number may come back a hair off

        /// <summary>The buyer names a price, as a fraction of the factory's price. A raise is always welcome.</summary>
        public BargainReply Offer(double fraction)
        {
            if (Stock == null) throw new InvalidOperationException("Barangnya sudah habis. Datang lagi nanti.");
            if (fraction <= 0) throw new InvalidOperationException("Sebut harga di atas nol.");
            if (Agreed) return fraction >= Ask ? BargainReply.Deal : BargainReply.Final;   // a struck price is not reopened
            if (fraction >= Ask || Patience > 0 && fraction >= floor)
            {
                Ask = Math.Min(Ask, fraction); Agreed = true; return BargainReply.Deal;
            }
            if (Patience == 0) return BargainReply.Final;
            bool offended = fraction < Ask * rules.Insult;
            Patience = Math.Max(0, Patience - (offended ? 2 : 1));
            if (!offended) Ask -= (Ask - floor) * rules.Concede;
            return offended ? BargainReply.Offended : Patience == 0 ? BargainReply.Final : BargainReply.Counter;
        }

        internal SellerSave Snapshot() => new SellerSave
        {
            id = Id, round = Round, kg = Stock?.GrossKg ?? 0, claimed = ClaimedKg, ask = Ask, patience = Patience,
            weighed = Weighed, agreed = Agreed, inspected = Stock?.KnownGrade.HasValue ?? false
        };

        /// <summary>Brings back how a door stood when the career was saved, so a reload neither restocks nor resets the talk.</summary>
        internal void Load(SellerSave s)
        {
            Open(s.round);
            double full = Stock.GrossKg;
            if (s.kg < 0 || s.kg > full || s.claimed < s.kg - Slack || s.ask <= 0 || s.ask > Ask + Slack || s.patience < 0 ||
                s.patience > Habits.Patience) throw new FormatException("Simpanan penjual tidak valid.");
            Stock = s.kg == 0 ? null : s.kg < full ? Stock.Part(s.kg) : Stock;
            ClaimedKg = s.claimed; Ask = s.ask; Patience = s.patience; Weighed = s.weighed; Agreed = s.agreed;
            if (s.inspected) Stock?.Inspect();
        }
    }
}
