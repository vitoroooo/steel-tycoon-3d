using UnityEngine;

namespace Steel.Editor.Ground
{
    /// <summary>Heights: dead flat where the game is played, rolling hills beyond the boundary so the horizon is never bare.</summary>
    internal static class GroundShape
    {
        private const float Shoulder = 10, Rise = 90;   // flat margin past the boundary, distance over which hills grow
        private const float HillLow = 5, HillHigh = 26;

        internal static float[,] Heights(GroundZones zones, int resolution)
        {
            var heights = new float[resolution, resolution];
            for (int row = 0; row < resolution; row++)
                for (int column = 0; column < resolution; column++)
                {
                    float x = GroundZones.Centre.x + ((float)column / (resolution - 1) - .5f) * GroundZones.Size;
                    float z = GroundZones.Centre.z + ((float)row / (resolution - 1) - .5f) * GroundZones.Size;
                    heights[row, column] = (GroundZones.Base + Hill(zones, x, z)) / GroundZones.Depth;
                }
            return heights;
        }

        private static float Hill(GroundZones zones, float x, float z)
        {
            float grow = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(Shoulder, Shoulder + Rise, zones.Outside(x, z)));
            if (grow <= 0) return 0;
            float ridges = Mathf.PerlinNoise(x * .011f + 40, z * .011f + 7) * .75f + Mathf.PerlinNoise(x * .05f, z * .05f + 90) * .25f;
            return grow * Mathf.Lerp(HillLow, HillHigh, ridges);
        }
    }
}
