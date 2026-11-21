using Steel.Core.State;
using Steel.Core.State.Slice;

namespace Steel.Core.Fleet
{
    /// <summary>Tuning of one drivable vehicle of PRD 5.1, read from its own config file.</summary>
    public sealed class MotorRules
    {
        public readonly string Id, Label;
        public readonly double CapacityKg, Price, TripRealSeconds, MaxSpeed, ReverseSpeed;
        public readonly double Acceleration, Braking, TurnDegrees, EnterRange, ExitMaxSpeed;
        public readonly WorldPoint Spawn;

        public MotorRules(string id, string label, string json)
        {
            Id = id; Label = label;
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
