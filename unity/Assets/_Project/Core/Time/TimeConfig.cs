using Steel.Core.State;

namespace Steel.Core.Time
{
    public sealed class TimeConfig
    {
        public double RealSecondsPerGameHour { get; }
        public double OfflineCapRealSeconds { get; }

        public TimeConfig(string json)
        {
            var values = new ConfigValues(json);
            RealSecondsPerGameHour = values.Positive("realSecondsPerGameHour");
            OfflineCapRealSeconds = values.Positive("offlineCapRealSeconds");
        }
    }
}
