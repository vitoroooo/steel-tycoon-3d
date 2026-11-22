using System;
using System.Collections.Generic;

namespace Steel.Core.State
{
    public sealed class ConfigValues
    {
        private readonly Dictionary<string, double> values;
        public ConfigValues(string json) { values = NumericJson.Parse(json); }

        public double Get(string key)
        {
            if (!values.TryGetValue(key, out double value))
                throw new FormatException("Missing configuration key: " + key);
            return value;
        }

        public double Positive(string key)
        {
            double value = Get(key);
            if (value <= 0) throw new FormatException("Expected positive value: " + key);
            return value;
        }
        public double Optional(string key, double fallback) => values.TryGetValue(key, out var v) ? v : fallback;

        public double Fraction(string key)
        {
            double value = Get(key);
            if (value < 0 || value >= 1)
                throw new FormatException("Expected fraction in [0,1): " + key);
            return value;
        }

        public int Integer(string key)
        {
            double value = Get(key);
            if (value < 0 || value > int.MaxValue || value != Math.Floor(value))
                throw new FormatException("Expected nonnegative integer: " + key);
            return (int)value;
        }
    }
}
