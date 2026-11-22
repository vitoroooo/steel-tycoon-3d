using System;
using Steel.Core.Events;
using Steel.Core.Scrap;
using Steel.Core.State.Slice;
using Steel.Core.Trade.Spk;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        private double[] SteelRates => new[] { Prices.Ibn, Prices.Ibn * millRules.Positive("beamRate"), Prices.Ibn * millRules.Positive("boltRate") };
        private void EventHour(long hour) => session.AdvanceEvents(hour, Prices.Ibn, SteelRates);

        public EventView Events
        {
            get
            {
                EventSave e = session.Events; long day = SpkRules.Day(GameHours);
                bool yard = session.Yard.bandar;
                return new EventView
                {
                    Note = e.note, Strike = yard && session.EventTerms.CollectorStrike(day), Supply = session.EventTerms.Supply(day),
                    Spike = session.PriceSpike(GameHours), SpikeRise = e.spike,
                    Dirty = e.DirtyWaiting, DirtyGrade = ScrapText.Grade((ScrapGrade)e.dirtGrade), DirtyKg = e.dirtKg, Dirt = e.dirt,
                    DirtyPrice = e.dirtKg * (1 - e.dirt) * session.Yard.bids[e.dirtGrade],
                    DirtyHoursLeft = Math.Max(0, e.dirtUntil - GameHours),
                    Flash = e.flash && GameHours < e.flashUntil, FlashTaken = e.flashTaken,
                    FlashProduct = SliceSession.SteelName(e.flashProduct), FlashKg = e.flashKg, FlashSent = e.flashSent,
                    FlashPrice = e.flashPrice, FlashHoursLeft = Math.Max(0, e.flashUntil - GameHours),
                    FlashReady = session.Mill.goods[e.flashProduct]
                };
            }
        }

        public string AnswerDeposit(bool accept)
        {
            long hour = (long)Math.Floor(GameHours);
            if (accept) session.AcceptDirtyDeposit(business.Positive("capacityKg"), hour);
            else session.RejectDirtyDeposit(hour);
            return session.Events.note;
        }

        public string TakeFlashTender() { session.TakeFlashTender(GameHours); return session.Events.note; }
        public double DeliverFlashTender() => session.DeliverFlashTender(GameHours);
    }
}
