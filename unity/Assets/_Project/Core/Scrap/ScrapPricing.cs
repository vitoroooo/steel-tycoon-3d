using System;
using Steel.Core.State;

namespace Steel.Core.Scrap
{
    public sealed class ScrapPricing
    {
        private readonly ScrapConfig config;

        public ScrapPricing(ScrapConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public double Quote(ScrapPile pile, double referencePerKg)
        {
            if (pile == null) throw new ArgumentNullException(nameof(pile));
            NumberRules.NonNegative(referencePerKg, nameof(referencePerKg));
            ScrapGrade grade = pile.IsSorted ? pile.KnownGrade.Value : ScrapGrade.Mixed;
            return pile.SaleWeightKg * referencePerKg * config.Factor(grade, pile.ReuseQuality);
        }
    }
}
