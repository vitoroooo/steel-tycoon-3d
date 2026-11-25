using Steel.Core.State;
using Steel.Core.State.Slice;

namespace Steel.Core.Fleet
{
    /// <summary>Tuning for the gerobak dorong, the free starter vehicle (PRD 5.1: 50 kg).</summary>
    public sealed class CartRules
    {
        public readonly double CapacityKg, GrabRange, PushSpeed;
        public readonly WorldPoint Spawn;

        public CartRules(string json)
        {
            var values = new ConfigValues(json);
            CapacityKg = values.Positive("capacityKg");
            GrabRange = values.Positive("grabRange");
            PushSpeed = values.Positive("pushSpeedMps");
            Spawn = new WorldPoint(values.Get("spawnX"), values.Get("spawnZ"));
        }
    }
}
