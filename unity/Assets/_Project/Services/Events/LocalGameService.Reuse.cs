using System;
using Steel.Core.Events;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        public ReuseView Reuse
        {
            get
            {
                EventSave e = session.Events; double reference = Prices.ScrapReference;
                bool order = e.reuseOrder && GameHours < e.reuseOrderUntil;
                return new ReuseView
                {
                    Found = e.ReuseWaiting, Kg = e.reuseKg, Price = e.reuseKg * reference * session.ReuseFindRate,
                    HoursLeft = Math.Max(0, e.reuseUntil - GameHours),
                    StoreKg = e.reuseStore, StorePrice = e.reuseStore * reference * session.EventTerms.Get("reuseLow"),
                    Order = order, OrderKg = Math.Min(e.reuseStore, e.reuseOrderKg),
                    OrderPay = Math.Min(e.reuseStore, e.reuseOrderKg) * e.reuseOrderPrice,
                    OrderHoursLeft = order ? e.reuseOrderUntil - GameHours : 0
                };
            }
        }

        public string AnswerReuse(bool sell)
        {
            if (sell) session.SellReuseFind(Prices.ScrapReference); else session.KeepReuseFind();
            return session.Events.note;
        }

        public double DeliverReuseOrder() => session.DeliverReuseOrder(GameHours);
        public double SellReuseStore() => session.SellReuseStore(Prices.ScrapReference);
    }
}
