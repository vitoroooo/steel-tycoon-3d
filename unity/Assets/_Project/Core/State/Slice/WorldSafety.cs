using System;
using Steel.Core.State;

namespace Steel.Core.State.Slice
{
    public sealed class WorldSafety
    {
        public double SpawnY { get; }
        public double FallY { get; }
        public double HalfWidth { get; }
        public double MinZ { get; }
        public double MaxZ { get; }
        public WorldSafety(string json)
        {
            var values = new ConfigValues(json);
            SpawnY = values.Get("spawnY"); FallY = values.Get("respawnBelowY");
            HalfWidth = values.Positive("boundaryHalfWidth");
            MinZ = values.Get("boundaryMinZ"); MaxZ = values.Get("boundaryMaxZ");
            if (FallY >= SpawnY || MinZ >= MaxZ) throw new FormatException("Invalid world safety bounds.");
        }
        public bool NeedsRespawn(double y) => double.IsNaN(y) || double.IsInfinity(y) || y < FallY;
    }
}
