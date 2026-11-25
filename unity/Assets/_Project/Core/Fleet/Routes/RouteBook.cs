using System;
using System.Collections.Generic;
using System.Linq;
using Steel.Core.Scrap;
using Steel.Core.State.Saving;
using Steel.Core.State;

namespace Steel.Core.Fleet.Routes
{
    public sealed class RouteBook
    {
        private readonly List<RouteOrder> orders = new List<RouteOrder>();
        private readonly Dictionary<int, double> reserved = new Dictionary<int, double>();
        private long day;
        public IReadOnlyList<RouteOrder> Orders => orders;
        public double Reserved(int district) => reserved.TryGetValue(district, out var kg) ? kg : 0;
        public bool Busy(string carrier) => orders.Any(o => o.Carrier == carrier && o.SecondsLeft > 0);

        internal OrderSave[] Snapshot() => orders.Select(o => new OrderSave { id = o.Carrier, district = o.District,
            bought = o.Purchased, remaining = o.SecondsLeft, kg = o.KgLeft, unitCost = o.UnitCost }).ToArray();

        internal void Load(OrderSave[] saved, double[] quotas, long currentDay)
        {
            orders.Clear(); reserved.Clear(); day = currentDay;
            for (int i = 0; i < quotas.Length; i++) reserved[i] = NumberRules.NonNegative(quotas[i], "quota");
            foreach (var s in saved)
            {
                if (s.district < 0 || s.district >= quotas.Length || s.kg <= 0) throw new FormatException("Rute rusak");
                var kg = NumberRules.NonNegative(s.kg, "kg");
                orders.Add(new RouteOrder(s.id, s.district, NumberRules.NonNegative(s.remaining, "remaining"),
                    kg, kg * NumberRules.NonNegative(s.unitCost, "cost"), s.bought));
            }
        }

        public void Book(string carrier, int district, double seconds, double kg, double cost, bool purchased)
        {
            orders.Add(new RouteOrder(carrier, district, seconds, kg, cost, purchased));
            reserved[district] = Reserved(district) + kg;
        }

        public void OnHour(long hour)
        {
            if (hour / 24 == day) return;
            day = hour / 24; reserved.Clear();
        }

        public void Advance(double seconds)
        {
            var consumed = new Dictionary<string, double>();
            foreach (var order in orders)
            {
                if (order.SecondsLeft <= 0) continue;
                consumed.TryGetValue(order.Carrier, out var used);
                consumed[order.Carrier] = used + order.Advance(seconds - used);
            }
        }

        public double Unload(Backpack bag)
        {
            var order = orders.Find(o => o.SecondsLeft == 0)
                ?? throw new InvalidOperationException("Belum ada kiriman yang tiba.");
            double kg = Math.Min(bag.CapacityKg - bag.CarriedKg, order.KgLeft);
            if (kg <= .001) throw new InvalidOperationException("Muatan penuh. Jual dahulu.");
            var pile = new ScrapPile(kg, order.Purchased ? ScrapGrade.Hms1 : ScrapGrade.Mixed,
                acquisitionCost: kg * order.UnitCost);
            pile.Inspect(); bag.Add(pile); order.Take(kg);
            if (order.KgLeft <= .001) orders.Remove(order);
            return kg;
        }
    }
}
