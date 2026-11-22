using System;
using Steel.Core.State.Slice;

namespace Steel.Core.Weather
{
    public static class DistrictWeather
    {
        public static int District(WorldPoint p) => p.Z < -16 ? 2 : p.Z > 25 ? 1 : 0;
        public static bool Raining(long hour, int district)
        {
            if (hour <= 0) return false;
            long day = hour / 24;
            uint mix = unchecked((uint)(day * 747796405 + district * 2891336453));
            mix = (mix ^ mix >> 16) * 2246822519u; mix ^= mix >> 13;
            int start = day == 0 ? 1 + district * 5 : (int)(mix % 14);
            int length = day == 0 ? 8 : (int)(mix / 16 % 12) + 1;
            return hour % 24 >= start && hour % 24 < start + length;
        }
        public static int Soil(long hour, int district)
        {
            if (!Raining(hour,district)) return 0;
            for (int h = 1; h <= 6; h++) if (!Raining(hour-h,district)) return 1;
            return 2;
        }
        public static int RainHours(long start, int district)
        {
            int count = 0;
            for (int i = 0; i < 24; i++) if (Raining(start+i,district)) count++;
            return count;
        }
        public static double ValueFactor(int exposure) => Math.Pow(.85, exposure / 48);
    }
}
