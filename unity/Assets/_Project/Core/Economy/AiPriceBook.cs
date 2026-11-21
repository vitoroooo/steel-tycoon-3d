using System;
using Steel.Core.State;

namespace Steel.Core.Economy
{
    public sealed class AiPriceBook
    {
        private readonly EconomyConfig config;
        private readonly IbnIndex index;
        private double lastRequest;
        public long CurrentBucket { get; private set; } = -1;
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
            double scaled = realSeconds / config.RefreshRealSeconds;
            double nearest = Math.Round(scaled);
            if (Math.Abs(scaled - nearest) <= 1e-12 * Math.Max(1, Math.Abs(scaled))) scaled = nearest;
            if (scaled >= long.MaxValue) throw new ArgumentOutOfRangeException(nameof(realSeconds));
            bool refreshed = RefreshBucket((long)Math.Floor(scaled), cityIngotKg);
            lastRequest = realSeconds;
            return refreshed;
        }

        public bool RefreshBucket(long bucket, double cityIngotKg)
        {
            NumberRules.NonNegative(cityIngotKg, nameof(cityIngotKg));
            if (bucket < 0 || bucket < CurrentBucket) throw new ArgumentOutOfRangeException(nameof(bucket));
            double time = bucket * config.RefreshRealSeconds;
            NumberRules.NonNegative(time, nameof(bucket));
            if (bucket == CurrentBucket) return false;
            Current = new AiQuote(time, index.Evaluate(time, cityIngotKg), config);
            CurrentBucket = bucket;
            lastRequest = Math.Max(lastRequest, time);
            return true;
        }
    }
}
