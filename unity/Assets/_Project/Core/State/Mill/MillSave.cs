using System;
using System.Linq;

namespace Steel.Core.State.Saving
{
    [Serializable] public sealed class MillSave
    {
        public bool filed, opened, rolling, bolts, shredder;
        public double permitAt, completedAt, pendingKg;
        public int product;
        public double[] goods = new double[3];

        internal static MillSave Restore(MillSave s)
        {
            if (s == null || !s.filed && (s.goods == null || s.goods.Length == 0)) return new MillSave();
            if (s.goods?.Length != 3 || s.goods.Any(v => !Valid(v)) ||
                !Valid(s.permitAt) || !Valid(s.completedAt) || !Valid(s.pendingKg) ||
                s.product < 0 || s.product > 2 || s.opened && !s.filed ||
                !s.opened && (s.rolling || s.bolts || s.shredder || s.pendingKg > 0 || s.goods.Any(v => v > 0)))
                throw new FormatException("Data pabrik tidak valid.");
            return s;
        }
        private static bool Valid(double v) => !double.IsNaN(v) && !double.IsInfinity(v) && v >= 0;
        internal void Finish(double hour)
        {
            if (pendingKg <= 0 || hour < completedAt) return;
            goods[product] += pendingKg;
            pendingKg = 0;
        }
    }
}
