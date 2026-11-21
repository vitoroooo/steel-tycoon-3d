namespace Steel.Services
{
    /// <summary>What the presentation layer needs to draw and steer the gerobak.</summary>
    public sealed class CartView
    {
        public bool Held, Nearby;
        public float X, Z, PushSpeed;
        public double CargoKg, CapacityKg;
    }
}
