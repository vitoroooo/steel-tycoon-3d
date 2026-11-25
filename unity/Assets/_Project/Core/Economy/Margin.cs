using Steel.Core.State;

namespace Steel.Core.Economy
{
    public static class Margin
    {
        public static double PerSaleKg(double buy, double sell, double operatingCost = 0,
            double lossFraction = 0, double taxFraction = 0)
        {
            NumberRules.NonNegative(buy, nameof(buy));
            NumberRules.NonNegative(sell, nameof(sell));
            NumberRules.NonNegative(operatingCost, nameof(operatingCost));
            NumberRules.Fraction(lossFraction, nameof(lossFraction));
            NumberRules.Fraction(taxFraction, nameof(taxFraction));
            return sell * (1 - taxFraction) - buy / (1 - lossFraction) - operatingCost;
        }
    }
}
