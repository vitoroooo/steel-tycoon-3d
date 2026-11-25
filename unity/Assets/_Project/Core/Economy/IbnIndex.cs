using System;
using Steel.Core.State;

namespace Steel.Core.Economy
{
    public sealed class IbnIndex
    {
        private readonly EconomyConfig config;

        public IbnIndex(EconomyConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public double Evaluate(double realSeconds, double cityIngotKg)
        {
            NumberRules.NonNegative(realSeconds, nameof(realSeconds));
            NumberRules.NonNegative(cityIngotKg, nameof(cityIngotKg));
            double day = Math.Floor(realSeconds / TimeSpan.FromDays(1).TotalSeconds);
            int seed = unchecked(config.NoiseSeed + (int)(day % int.MaxValue));
            double noise = (new Random(seed).NextDouble() * 2 - 1) * config.DailyNoiseFraction;
            double cycle = 1 + config.CycleAmplitude *
                Math.Sin(2 * Math.PI * (realSeconds % config.CycleRealSeconds) / config.CycleRealSeconds);
            double effect = (1 - cityIngotKg / config.SupplyTargetKg) * config.SupplyLimit;
            effect = Math.Max(-config.SupplyLimit, Math.Min(config.SupplyLimit, effect));
            return config.BaseIbn * cycle * (1 + noise) * (1 + effect);
        }
    }
}
