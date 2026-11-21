namespace Steel.Services
{
    public struct VehicleView
    {
        public string Id, Label;
        public bool Owned, Occupied, Nearby;
        public float? Heading;
        public float X, Z, Speed, MaxSpeed, ReverseSpeed, Acceleration, Braking, TurnDegrees;
        public double Price, CargoKg, CapacityKg, RemainingTripSeconds;
    }
}
