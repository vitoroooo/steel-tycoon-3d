using System;
using Steel.Core.State.Saving;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public TenderSave Tender { get; private set; } = new TenderSave();
        public void BidTender(double hour,double bid,bool certified)
        {
            RequireNear(Rules.Factory);
            long round = TenderSave.Current(hour);
            if (!Mill.rolling || BusinessLevel < 40 || Yard.reputation < 60 || Tender.awarded ||
                Tender.offered && Tender.round == round || hour % TenderSave.Interval >= TenderSave.Closing)
                throw new InvalidOperationException("Perlu level 40, lini rolling, reputasi 60 dan lelang terbuka.");
            if (NumberRules.NonNegative(bid,"bid") <= 0) throw new InvalidOperationException("Harga tidak valid.");
            Tender = new TenderSave { offered = true,round = round,bid = bid,repAtBid = Yard.reputation,
                certified = certified,last = "Penawaran disegel sampai penutupan." };
        }
        public void ResolveTender(double hour,double reference)
        {
            var t = Tender;
            if (!t.offered) return;
            double age = hour-t.round*TenderSave.Interval;
            if (!t.awarded && age >= TenderSave.Closing && t.sent == 0 && t.bid > 0)
            {
                t.awarded = t.Score(reference);
                t.last = t.awarded ? "Tender dimenangi. Kirim H-beam ke proyek jembatan." : "Tender dimenangi pesaing.";
                if (!t.awarded) t.bid = 0;
            }
            if (t.awarded && age > TenderSave.Deadline)
            {
                t.awarded = false; t.bid = 0; Yard.reputation = Math.Max(0,Yard.reputation-5);
                t.last = "Tenggat terlewat. Tonase yang terkirim dibayar; reputasi turun.";
            }
        }
        public double DeliverTender()
        {
            RequireNear(Rules.Factory);
            var t = Tender;
            if (!t.awarded || Mill.goods[1] <= 0) throw new InvalidOperationException("Tidak ada pengiriman H-beam.");
            double kg = Math.Min(t.Remaining,Mill.goods[1]); double payment = t.bid*kg;
            CollectSteelProceeds(1,kg,payment); t.sent += kg;
            if (t.Remaining <= 0)
            {
                t.awarded = false; t.bid = 0; Yard.reputation = Math.Min(100,Yard.reputation+5);
                t.last = "Jembatan selesai. Reputasi +5.";
            }
            return payment;
        }
    }
}
