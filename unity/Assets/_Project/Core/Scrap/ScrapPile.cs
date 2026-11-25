using System;
using Steel.Core.State;
using Steel.Core.State.Saving;

namespace Steel.Core.Scrap
{
    public sealed class ScrapPile
    {
        private readonly ScrapGrade hiddenGrade;
        private readonly double dirtFraction;
        internal ScrapGrade Material => hiddenGrade;
        public double WasteKg => GrossKg * dirtFraction;
        internal int RainHours { get; private set; }
        public double RustFactor => Weather.DistrictWeather.ValueFactor(RainHours);
        internal void WetHour()
        {
            if (Material == ScrapGrade.Hms1 || Material == ScrapGrade.Reuse) RainHours++;
        }
        internal void RestoreRain(int h) { if (h < 0) throw new FormatException("Karat tidak valid."); RainHours = h; }
        public double GrossKg { get; }
        public double AcquisitionCost { get; }
        public double ReuseQuality { get; }
        public ScrapGrade? KnownGrade { get; private set; }
        public bool IsSorted { get; private set; }
        public double SaleWeightKg => IsSorted ? GrossKg * (1 - dirtFraction) : GrossKg;

        public ScrapPile(double grossKg, ScrapGrade hiddenGrade, double dirtFraction = 0,
            double reuseQuality = 0, double acquisitionCost = 0)
        {
            GrossKg = NumberRules.NonNegative(grossKg, nameof(grossKg));
            AcquisitionCost = NumberRules.NonNegative(acquisitionCost, nameof(acquisitionCost));
            if (!Enum.IsDefined(typeof(ScrapGrade), hiddenGrade))
                throw new ArgumentOutOfRangeException(nameof(hiddenGrade));
            this.hiddenGrade = hiddenGrade;
            this.dirtFraction = NumberRules.NonNegative(dirtFraction, nameof(dirtFraction));
            ReuseQuality = NumberRules.NonNegative(reuseQuality, nameof(reuseQuality));
            if (dirtFraction > 1 || reuseQuality > 1) throw new ArgumentOutOfRangeException();
        }

        internal ScrapPile Fresh() => new ScrapPile(GrossKg, hiddenGrade, dirtFraction, ReuseQuality);

        internal LotSave Snapshot() => new LotSave { grade = (int)hiddenGrade, kg = GrossKg,
            dirt = dirtFraction, quality = ReuseQuality, cost = AcquisitionCost,
            inspected = KnownGrade.HasValue, sorted = IsSorted, rainHours = RainHours };

        internal static ScrapPile Load(LotSave dto)
        {
            var p = new ScrapPile(dto.kg, (ScrapGrade)dto.grade, dto.dirt, dto.quality, dto.cost);
            p.RestoreRain(dto.rainHours);
            if (dto.inspected) p.Inspect();
            if (dto.sorted) p.Sort();
            return p;
        }

        public ScrapGrade Inspect()
        {
            KnownGrade = hiddenGrade;
            return hiddenGrade;
        }

        public void Sort()
        {
            if (!KnownGrade.HasValue) throw new InvalidOperationException("Inspect before sorting.");
            IsSorted = true;
        }
    }
}
