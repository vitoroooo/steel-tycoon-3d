namespace Steel.Services
{
    public sealed class VehicleView
    {
        public string Id, Label;
        public bool Owned, Occupied, Nearby;
        public float X, Z, Speed, MaxSpeed, ReverseSpeed, Acceleration, Braking, TurnDegrees;
        public double Price, CargoKg, CapacityKg, RemainingTripSeconds;
    }
}
