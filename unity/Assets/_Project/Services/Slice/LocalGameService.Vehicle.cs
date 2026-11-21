using Steel.Core.State.Slice;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        public VehicleView Vehicle
        {
            get
            {
                var car = session.Pickup; var rules = car.Rules;
                return new VehicleView
                {
                    Occupied = car.Occupied, Nearby = session.Position.Distance(car.Position) <= rules.EnterRange,
                    X = (float)car.Position.X, Z = (float)car.Position.Z, Speed = (float)car.Speed,
                    MaxSpeed = (float)rules.MaxSpeed, ReverseSpeed = (float)rules.ReverseSpeed,
                    Acceleration = (float)rules.Acceleration, Braking = (float)rules.Braking,
                    TurnDegrees = (float)rules.TurnDegrees, CargoKg = car.Cargo.CarriedKg,
                    CapacityKg = rules.CapacityKg, RemainingTripSeconds = car.RemainingTripSeconds
                };
            }
        }
        public void EnterVehicle() { session.EnterPickup(); pending = null; }
        public void ExitVehicle(float x, float z)
        { session.ExitPickup(new WorldPoint(x, z)); pending = null; }
        public void ReportVehicle(float x, float z, float speed) =>
            session.ReportPickup(new WorldPoint(x, z), speed);
    }
}
