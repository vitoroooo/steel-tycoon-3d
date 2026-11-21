using System;
using Steel.Core.State;

namespace Steel.Core.Economy
{
    public sealed class AiPriceBook
    {
        private readonly EconomyConfig config;
        private readonly IbnIndex index;
        private double lastRequest;
        public AiQuote Current { get; private set; }

        public AiPriceBook(EconomyConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            index = new IbnIndex(config);
        }

        public bool Refresh(double realSeconds, double cityIngotKg)
        {
            NumberRules.NonNegative(realSeconds, nameof(realSeconds));
            NumberRules.NonNegative(cityIngotKg, nameof(cityIngotKg));
            if (realSeconds < lastRequest) throw new ArgumentOutOfRangeException(nameof(realSeconds));
            double time = Math.Floor(realSeconds / config.RefreshRealSeconds) * config.RefreshRealSeconds;
            lastRequest = realSeconds;
            if (Current != null && Current.RefreshedAtRealSeconds == time) return false;
            Current = new AiQuote(time, index.Evaluate(time, cityIngotKg), config);
            return true;
        }
    }
}
