using System;
using System.Collections.Generic;
using System.Linq;
using Steel.Core.Weather;

namespace Steel.Core.State.Saving
{
    [Serializable] public sealed class SteelBatch
    {
        public double kg;
        public int rain;
    }
    public sealed partial class YardSave
    {
        public bool hangar, paved, stuck, rescued;
        public int entries;
        public double rescueHour, payout, dispatchedKg, unloadHour;
        public List<SteelBatch> exposed = new List<SteelBatch>();
        internal void RestoreWeather()
        {
            if (exposed == null) exposed = new List<SteelBatch>();
            if (exposed.Count == 0 && stock[0] > 0) exposed.Add(new SteelBatch { kg = stock[0] });
            if (exposed.Any(b => b == null || b.rain < 0 || !(b.kg > 0)) ||
                Math.Abs(stock[0] - exposed.Sum(b => b.kg)) > .001) throw new FormatException("Stok HMS tidak cocok.");
            NumberRules.NonNegative(payout,"payout"); NumberRules.NonNegative(unloadHour,"unload");
        }
        internal void WetStock()
        {
            double space = hangar ? 50000 : 0;
            for (int i = 0; i < exposed.Count; i++)
            {
                var b = exposed[i];
                if (space >= b.kg) { space -= b.kg; continue; }
                if (space > 0)
                {
                    exposed.Insert(i++,new SteelBatch { kg = space,rain = b.rain });
                    b.kg -= space; space = 0;
                }
                b.rain++;
            }
        }
        public double BillableKg(int grade,double kg) => InvoiceWeight(grade,kg,false);
        internal double InvoiceWeight(int grade, double kg, bool take)
        {
            if (grade != 0) return kg;
            double left = kg, value = 0;
            foreach (var b in exposed)
            {
                double part = Math.Min(left,b.kg);
                value += part * DistrictWeather.ValueFactor(b.rain); left -= part;
                if (take) b.kg -= part;
                if (left <= 0) break;
            }
            if (take) exposed.RemoveAll(b => b.kg <= 0);
            return value + left;
        }
    }
}
