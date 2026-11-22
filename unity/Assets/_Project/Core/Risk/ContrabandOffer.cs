using System;
using Steel.Core.Trade.Street;

namespace Steel.Core.Risk
{
    /// <summary>A shady load offered at the yard for one game day (PRD 6.7). The same offer for everyone on the same day.</summary>
    public readonly struct ContrabandOffer
    {
        private static readonly string[] Names = { "Kabel listrik tanpa surat", "Rel bekas tanpa asal-usul", "Aset sitaan bermasalah" };
        public readonly long Day;
        public readonly string Name;
        /// <summary>Its legal worth, what it earns if nobody looks, and the fine if they do.</summary>
        public readonly double Kg, Value, Gain, Fine;

        private ContrabandOffer(long day, string name, double kg, double value, double gain, double fine)
        {
            Day = day; Name = name; Kg = kg; Value = value; Gain = gain; Fine = fine;
        }

        /// <param name="perKg">The day's reference price of clean heavy scrap.</param>
        public static ContrabandOffer? On(long day, double perKg, RiskRules rules)
        {
            double Roll(int salt) => SellerRoll.Of(1, day, 700 + salt);
            if (Roll(0) >= rules.Get("offerChance")) return null;
            double kg = Math.Round((rules.Get("offerMinKg") + Roll(1) * (rules.Get("offerMaxKg") - rules.Get("offerMinKg"))) / 50) * 50;
            double margin = rules.Get("marginMin") + Roll(2) * (rules.Get("marginMax") - rules.Get("marginMin"));
            double value = kg * perKg;
            return new ContrabandOffer(day, Names[(int)(Roll(3) * Names.Length)], kg, value,
                Math.Round(value * rules.Get("legalMargin") * margin), value * rules.Get("fineFactor"));
        }
    }
}
