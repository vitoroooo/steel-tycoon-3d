using Steel.Core.State;
using Steel.Core.State.Slice;

namespace Steel.Core.Fleet
{
    public sealed class PickupRules
    {
        public readonly double CapacityKg, Price, TripRealSeconds, MaxSpeed, ReverseSpeed;
        public readonly double Acceleration, Braking, TurnDegrees, EnterRange, ExitMaxSpeed;
        public readonly WorldPoint Spawn;
        public PickupRules(string json)
        {
            var v = new ConfigValues(json);
            CapacityKg = v.Positive("capacityKg"); Price = v.Positive("price");
            TripRealSeconds = v.Positive("tripRealSeconds");
            MaxSpeed = v.Positive("maxSpeedMps"); ReverseSpeed = v.Positive("reverseSpeedMps");
            Acceleration = v.Positive("acceleration"); Braking = v.Positive("braking");
            TurnDegrees = v.Positive("turnDegrees"); EnterRange = v.Positive("enterRange");
            ExitMaxSpeed = v.Positive("exitMaxSpeed");
            Spawn = new WorldPoint(v.Get("spawnX"), v.Get("spawnZ"));
        }
    }
}
