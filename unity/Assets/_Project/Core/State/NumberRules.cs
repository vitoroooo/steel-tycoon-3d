using System;

namespace Steel.Core.State
{
    public static class NumberRules
    {
        public static double NonNegative(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0)
                throw new ArgumentOutOfRangeException(name);
            return value;
        }

        public static double Fraction(double value, string name)
        {
            NonNegative(value, name);
            if (value >= 1) throw new ArgumentOutOfRangeException(name);
            return value;
        }
    }
}
