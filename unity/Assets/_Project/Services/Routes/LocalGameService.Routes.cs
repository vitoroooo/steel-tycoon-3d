using System;
using System.Linq;
using Steel.Core.State;
using Steel.Core.Fleet.Routes;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        private static readonly string[] Areas = { "Kampung", "Pasar Lama", "Proyek Bongkaran" };
        private ConfigValues routeConfig;
        private double queueLimit;
        private RouteTicket Ticket(string id, int district, bool purchase)
        {
            var vehicle = id == "cart" ? null : session.Fleet.Find(id);
            int range = id == "cart" ? 1 : id == FleetCatalog.Triseda ? 2 : 3;
            if (district < 0 || district >= range)
                throw new InvalidOperationException("Di luar jangkauan armada.");
            double capacity = vehicle?.Rules.CapacityKg ?? session.Cart.Rules.CapacityKg;
            double kg = Math.Min(capacity * (purchase ? 1 : routeConfig.Fraction("freeLoadFraction")),
                routeConfig.Positive("district" + district + "Kg") - session.Routes.Reserved(district));
            if (purchase) kg = Math.Min(kg, Math.Floor(Math.Max(0, session.Cash -
                (vehicle == null ? 0 : routeConfig.Positive(id + "Fuel"))) / Prices.SellToK1));
            double seconds = vehicle?.Rules.TripRealSeconds ?? routeConfig.Positive("cartSeconds");
            return new RouteTicket(id, district, purchase, kg, seconds, purchase ? kg * Prices.SellToK1 : 0,
                vehicle == null ? 0 : routeConfig.Positive(id + "Fuel"));
        }
        public RouteView Route(string carrier, int district, bool purchased)
        {
            var t = Ticket(carrier, district, purchased);
            var orders = session.Routes.Orders;
            var next = orders.Where(o => o.SecondsLeft > 0).GroupBy(o => o.Carrier)
                .Select(g => g.First()).OrderBy(o => o.SecondsLeft).FirstOrDefault();
            return new RouteView { Carrier = carrier, District = district, Area = Areas[district], Purchased = purchased,
                Kg = t.Kg, Goods = t.Goods, Travel = t.Travel, Minutes = t.Seconds / 60,
                DepotKg = orders.Where(o => o.SecondsLeft == 0).Sum(o => o.KgLeft),
                Queue = orders.Count(o => o.SecondsLeft > 0), ArrivalMinutes = (next?.SecondsLeft ?? 0) / 60,
                NextCarrier = next?.Carrier };
        }
        public void BookRoute(RouteView preview)
        {
            if (preview == null) throw new InvalidOperationException("Periksa biaya rute dahulu.");
            var current = Ticket(preview.Carrier, preview.District, preview.Purchased);
            if (current.Kg != preview.Kg || current.Goods != preview.Goods || current.Travel != preview.Travel)
                throw new InvalidOperationException("Penawaran berubah. Periksa biaya terbaru.");
            session.Dispatch(current, queueLimit);
        }
        public double CollectRoute() => session.CollectRoute();
    }
}
