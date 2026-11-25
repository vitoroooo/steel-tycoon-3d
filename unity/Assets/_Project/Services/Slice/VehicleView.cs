namespace Steel.Services
{
    public sealed class VehicleView
    {
        public bool Occupied, Nearby;
        public float X, Z, Speed, MaxSpeed, ReverseSpeed, Acceleration, Braking, TurnDegrees;
        public double CargoKg, CapacityKg, RemainingTripSeconds;
    }
}
