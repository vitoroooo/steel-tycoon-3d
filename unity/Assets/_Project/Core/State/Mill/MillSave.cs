using System;
using System.Linq;

namespace Steel.Core.State.Saving
{
    [Serializable] public sealed class MillSave
    {
        public bool filed, opened, rolling, bolts, shredder, rfid;
        public double plantValue, gateReady, safetyReady;
        public double permitAt, completedAt, pendingKg;
        public int product;
        public double[] goods = new double[3];
        public double heatCost;
        public double[] costs = new double[3];

        internal static MillSave Restore(MillSave s)
        {
            if (s == null || !s.filed && (s.goods == null || s.goods.Length == 0)) return new MillSave();
            if (s.costs == null || s.costs.Length == 0) s.costs = new double[3];
            if (s.costs.Length != 3 || s.costs.Any(v => !Valid(v)) || !Valid(s.heatCost) ||
                s.goods?.Length != 3 || s.goods.Any(v => !Valid(v)) ||
                !Valid(s.permitAt) || !Valid(s.completedAt) || !Valid(s.pendingKg) ||
                !Valid(s.plantValue) || !Valid(s.gateReady) || !Valid(s.safetyReady) ||
                s.product < 0 || s.product > 2 || s.opened && !s.filed ||
                !s.opened && (s.rolling || s.bolts || s.shredder || s.rfid || s.safetyReady > 0 ||
                    s.plantValue > 0 || s.pendingKg > 0 || s.goods.Any(v => v > 0)))
                throw new FormatException("Data pabrik tidak valid.");
            return s;
        }
        private static bool Valid(double v) => !double.IsNaN(v) && !double.IsInfinity(v) && v >= 0;
        internal void Finish(double hour)
        {
            if (pendingKg <= 0 || hour < completedAt) return;
            goods[product] += pendingKg;
            costs[product] += heatCost; heatCost = pendingKg = 0;
        }
    }
}
