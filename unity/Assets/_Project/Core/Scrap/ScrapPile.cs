using System;
using Steel.Core.State;

namespace Steel.Core.Scrap
{
    public sealed class ScrapPile
    {
        private readonly ScrapGrade hiddenGrade;
        private readonly double dirtFraction;
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
