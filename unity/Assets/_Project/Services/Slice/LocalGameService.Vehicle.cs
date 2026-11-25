using Steel.Core.Fleet;
using Steel.Core.State.Slice;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        public VehicleView Vehicle =>
            Describe(session.Fleet.Driven ?? session.Fleet.Nearest(session.Position));

        public VehicleView VehicleOf(string id) => Describe(session.Fleet.Find(id));

        private VehicleView Describe(MotorState car)
        {
            var rules = car.Rules;
            return new VehicleView
            {
                Id = rules.Id, Label = rules.Label, Owned = car.Owned, Price = rules.Price,
                Occupied = car.Occupied, Nearby = car.InReach(session.Position),
                X = (float)car.Position.X, Z = (float)car.Position.Z, Speed = (float)car.Speed, Heading = (float?)car.Heading,
                MaxSpeed = (float)rules.MaxSpeed, ReverseSpeed = (float)rules.ReverseSpeed,
                Acceleration = (float)rules.Acceleration, Braking = (float)rules.Braking,
                TurnDegrees = (float)rules.TurnDegrees, CargoKg = car.Cargo.CarriedKg,
                CapacityKg = rules.CapacityKg, RemainingTripSeconds = car.RemainingTripSeconds
            };
        }

        public void BuyVehicle(string id) { session.BuyMotor(id); pending = null; }
        public void EnterVehicle(string id) { session.EnterMotor(id); pending = null; }
        public void ExitVehicle(float x, float z)
        { session.ExitMotor(new WorldPoint(x, z)); pending = null; }
        public void ReportVehicle(string id, float x, float z, float speed, float? heading = null) =>
            session.ReportMotor(id, new WorldPoint(x, z), speed, heading);
    }
}
