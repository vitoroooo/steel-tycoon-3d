using System;
using System.Collections.Generic;
using Steel.Core.State;

namespace Steel.Core.Scrap
{
    public sealed class ScrapConfig
    {
        private readonly Dictionary<ScrapGrade, double> factors;
        public double ReuseMin { get; }
        public double ReuseMax { get; }

        public ScrapConfig(string json)
        {
            var v = new ConfigValues(json);
            factors = new Dictionary<ScrapGrade, double>
            {
                { ScrapGrade.Hms1, v.Positive("hms1") },
                { ScrapGrade.Hms2, v.Positive("hms2") },
                { ScrapGrade.CastIron, v.Positive("castIron") },
                { ScrapGrade.Mixed, v.Positive("mixed") },
                { ScrapGrade.Dirt, NumberRules.NonNegative(v.Get("dirt"), "dirt") }
            };
            ReuseMin = v.Positive("reuseMin");
            ReuseMax = v.Positive("reuseMax");
            if (ReuseMax < ReuseMin) throw new FormatException("Invalid reuse price range.");
        }

        public double Factor(ScrapGrade grade, double reuseQuality)
        {
            NumberRules.NonNegative(reuseQuality, nameof(reuseQuality));
            if (reuseQuality > 1) throw new ArgumentOutOfRangeException(nameof(reuseQuality));
            if (grade == ScrapGrade.Reuse) return ReuseMin + reuseQuality * (ReuseMax - ReuseMin);
            if (!factors.TryGetValue(grade, out double value))
                throw new ArgumentOutOfRangeException(nameof(grade));
            return value;
        }
    }
}
