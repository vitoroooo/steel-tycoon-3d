using System;

namespace Steel.Core.Finance
{
    public static class FinanceRules
    {
        public const double RealDay = 86400;
        public const double Quarter = 15 * RealDay;
        public const double DailyInterest = .02;
        public const int MaxLevel = 50;

        public static double TaxRate(int caste)
        {
            switch (caste)
            {
                case 1: return .05;
                case 2: return .10;
                case 3: return .15;
                default: throw new ArgumentOutOfRangeException(nameof(caste));
            }
        }

        public static double QuarterTax(double profit, int caste) => Math.Max(0, profit) * TaxRate(caste);

        public static double Threshold(int level)
        {
            if (level < 1 || level > MaxLevel) throw new ArgumentOutOfRangeException(nameof(level));
            int step = level - 1;
            return 5000d * step * step * step;
        }

        public static double NextQuarter(double seconds) => (Math.Floor(seconds / Quarter) + 1) * Quarter;
    }
}
