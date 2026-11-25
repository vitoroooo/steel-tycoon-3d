using System;

namespace Steel.Services
{
    public sealed class FrameSample
    {
        private readonly int[] buckets = new int[1001];
        public int Frames { get; private set; }
        public double Elapsed { get; private set; }
        public double WorstMs { get; private set; }
        public int SlowFrames { get; private set; }
        public bool Finished => Elapsed >= 30;
        public double AverageFps => Elapsed > 0 ? Frames/Elapsed : 0;

        public void Add(double seconds)
        {
            if (Finished || seconds <= 0 || double.IsNaN(seconds) || double.IsInfinity(seconds)) return;
            double ms = seconds*1000;
            buckets[(int)Math.Min(1000,Math.Ceiling(ms))]++;
            Elapsed += seconds; Frames++; WorstMs = Math.Max(WorstMs,ms);
            if (ms > 50) SlowFrames++;
        }

        public int P95Ms()
        {
            int remaining = (int)Math.Ceiling(Frames*.95);
            for (int i = 0; i < buckets.Length; i++)
                if ((remaining -= buckets[i]) <= 0) return i;
            return 1000;
        }
    }
}
