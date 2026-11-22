using System.Linq;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        private double hourSeconds;
        public bool StreetTipRead { get; private set; }
        public void ReadStreetTip() => StreetTipRead = true;

        public IncomeView Income
        {
            get
            {
                var world = session.World;
                double room = session.Bag.CapacityKg - session.Bag.CarriedKg;
                var site = world.Piles.Where(p => !p.Collected).OrderBy(p => p.Scrap.GrossKg > room)
                    .ThenBy(p => p.Position.Distance(session.Position)).FirstOrDefault();
                var round = Route("cart", 0, false);
                double minute = hourSeconds / 60;
                return new IncomeView
                {
                    StreetSites = world.Available,
                    SiteX = (float)(site?.Position.X ?? 0), SiteZ = (float)(site?.Position.Z ?? 0),
                    SiteKg = site?.Scrap.GrossKg ?? 0,
                    RestockMinutes = world.Available == world.Piles.Count ? 0 :
                        world.HoursToRestock(GameHours) * minute,
                    RestockEveryMinutes = world.RestockHours * minute,
                    CartAwayMinutes = session.Routes.Orders.Where(o => o.Carrier == "cart")
                        .Sum(o => o.SecondsLeft) / 60,
                    DepotKg = round.DepotKg, ArrivalMinutes = round.ArrivalMinutes,
                    RouteKg = round.Kg, RouteMinutes = round.Minutes
                };
            }
        }
    }
}
