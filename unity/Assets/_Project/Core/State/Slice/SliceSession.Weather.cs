using System;
using Steel.Core.Scrap;
using Steel.Core.Weather;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public Backpack Covered { get; } = new Backpack(50);
        public void MoveCovered(bool store)
        {
            RequireNear(Rules.Home);
            var from = store ? Bag : Covered; var to = store ? Covered : Bag;
            if (from.Items.Count == 0) throw new InvalidOperationException("Tidak ada stok untuk dipindah.");
            int count = 0;
            foreach (var lot in from.Items)
            {
                if (to.CarriedKg + lot.GrossKg > to.CapacityKg) continue;
                to.Add(lot); count++;
            }
            if (count == 0) throw new InvalidOperationException("Kapasitas tujuan tidak cukup untuk satu tumpukan.");
            from.RemoveWhere(lot => Contains(to,lot));
        }
        private static bool Contains(Backpack bag, ScrapPile lot)
        {
            foreach (var p in bag.Items) if (p == lot) return true;
            return false;
        }
        public void WeatherHour(long hour)
        {
            bool Wet(WorldPoint p) => DistrictWeather.Raining(hour-1,DistrictWeather.District(p));
            void Age(Backpack bag, WorldPoint p)
            {
                if (Wet(p)) foreach (var pile in bag.Items) pile.WetHour();
            }
            foreach (var w in World.Piles)
                if (!w.Collected && Wet(w.Position)) w.Scrap.WetHour();
            Age(backpack,Position); Age(Cart.Cargo,Cart.Position);
            foreach (var motor in Fleet.All) Age(motor.Cargo,motor.Position);
            if (Wet(Rules.Home)) Yard.WetStock();
            PayShipment(hour);
        }
        public void PayShipment(double hour)
        {
            if (Yard.payout > 0 && hour >= Yard.unloadHour)
                CollectShipment();
        }
    }
}
