using System;
using System.Linq;
using Steel.Core.Fleet.Routes;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public RouteBook Routes { get; } = new RouteBook();

        public void Dispatch(RouteTicket ticket, double queueLimit)
        {
            RequireNear(Rules.Home);
            var car = ticket.Carrier == "cart" ? null : Fleet.Find(ticket.Carrier);
            bool available = car == null ? !Cart.Held && Cart.Cargo.CarriedKg == 0 :
                car.Owned && !car.Occupied && car.Cargo.CarriedKg == 0;
            if (!available) throw new InvalidOperationException("Armada harus dimiliki, kosong dan diparkir.");
            if (ticket.Kg <= 0) throw new InvalidOperationException("Pasokan habis. Coba kecamatan lain atau besok.");
            if (Routes.Orders.Where(r => r.Carrier == ticket.Carrier).Sum(r => r.SecondsLeft)
                + ticket.Seconds > queueLimit) throw new InvalidOperationException("Antrean armada sudah penuh.");
            if (ticket.Cost > Cash) throw new InvalidOperationException("Kas belum cukup.");
            Cash -= ticket.Cost;
            Routes.Book(ticket.Carrier, ticket.District, ticket.Seconds, ticket.Kg, ticket.Cost, ticket.Purchased);
        }

        public double CollectRoute()
        {
            RequireNear(Rules.Home);
            return Routes.Unload(Bag);
        }
    }
}
