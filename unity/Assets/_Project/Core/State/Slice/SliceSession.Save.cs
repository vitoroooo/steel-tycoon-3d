using System;
using System.Linq;
using Steel.Core.State.Saving;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public CareerSave Snapshot(int districts) => new CareerSave {
            version = 1, cash = Cash, x = Position.X, z = Position.Z, cartX = Cart.Position.X, cartZ = Cart.Position.Z,
            magnet = Magnet, xrf = Xrf, yard = Yard, mill = Mill, covered = Covered.Snapshot(),
            firstSale = FirstSaleMade, grant = GrantCollected, cartHeld = Cart.Held,
            bag = backpack.Snapshot(), cart = Cart.Cargo.Snapshot(),
            motors = Fleet.All.Select(m => m.Snapshot()).ToArray(),
            sites = World.Piles.Select(w => new SiteSave { id = w.Id, collected = w.Collected,
                inspected = w.Scrap.KnownGrade.HasValue, rainHours = w.Scrap.RainHours }).ToArray(),
            routes = Routes.Snapshot(), quota = Enumerable.Range(0, districts).Select(Routes.Reserved).ToArray()
        };

        public void Load(CareerSave s, int districts, long day)
        {
            if (s.version != 1 || s.grant && !s.firstSale || s.quota.Length != districts ||
                s.motors.Length != Fleet.All.Count || s.motors.Select(m => m.id).Distinct().Count() != s.motors.Length ||
                s.motors.Count(m => m.occupied) + (s.cartHeld ? 1 : 0) > 1) throw new FormatException("Karier tidak valid");
            Cash = NumberRules.NonNegative(s.cash, "cash"); FirstSaleMade = s.firstSale; GrantCollected = s.grant;
            Magnet = s.magnet; Xrf = s.xrf; Yard = YardSave.Restore(s.yard);
            Mill = MillSave.Restore(s.mill);
            Covered.Load(s.covered ?? Array.Empty<LotSave>());
            Position = new WorldPoint(s.x, s.z); Cart.Restore(new WorldPoint(s.cartX, s.cartZ), s.cartHeld);
            backpack.Load(s.bag); Cart.Cargo.Load(s.cart);
            foreach (var motor in s.motors) Fleet.Find(motor.id).Load(motor);
            foreach (var site in s.sites)
            {
                var w = World.Find(site.id);
                w.Scrap.RestoreRain(site.rainHours);
                if (site.inspected) w.Scrap.Inspect();
                w.Collected = site.collected;
            }
            Routes.Load(s.routes, s.quota, day);
        }
    }
}
