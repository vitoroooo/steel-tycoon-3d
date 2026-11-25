using System;

namespace Steel.Core.State.Slice
{
    public readonly struct WorldPoint
    {
        public double X { get; }
        public double Z { get; }
        public WorldPoint(double x, double z)
        {
            if (double.IsNaN(x) || double.IsInfinity(x) || double.IsNaN(z) || double.IsInfinity(z))
                throw new ArgumentOutOfRangeException();
            X = x; Z = z;
        }
        public double Distance(WorldPoint other) => Math.Sqrt(
            (X - other.X) * (X - other.X) + (Z - other.Z) * (Z - other.Z));
    }
}
