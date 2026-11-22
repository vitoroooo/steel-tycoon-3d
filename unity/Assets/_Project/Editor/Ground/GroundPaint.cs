using UnityEngine;

namespace Steel.Editor.Ground
{
    /// <summary>Blends the four ground layers per district: mud, wild grass, gravel and trodden kampung path.</summary>
    internal static class GroundPaint
    {
        internal const int Mud = 0, Grass = 1, Gravel = 2, Path = 3, Layers = 4;

        internal static float[,,] Weights(GroundZones zones, int resolution)
        {
            var map = new float[resolution, resolution, Layers];
            for (int row = 0; row < resolution; row++)
                for (int column = 0; column < resolution; column++)
                {
                    float x = GroundZones.Centre.x + ((column + .5f) / resolution - .5f) * GroundZones.Size;
                    float z = GroundZones.Centre.z + ((row + .5f) / resolution - .5f) * GroundZones.Size;
                    Blend(zones, x, z, out float grass, out float gravel, out float path);
                    map[row, column, Grass] = grass;
                    map[row, column, Gravel] = gravel;
                    map[row, column, Path] = path;
                    map[row, column, Mud] = 1 - grass - gravel - path;
                }
            return map;
        }

        private static void Blend(GroundZones zones, float x, float z, out float grass, out float gravel, out float path)
        {
            grass = zones.Grass(x, z);
            float bare = 1 - grass;
            // Gravel is spread over industrial yards but wears thin in patches, showing mud underneath.
            float spread = Mathf.InverseLerp(.3f, .5f, GroundZones.Patches(x, z, 53.7f));
            gravel = bare * zones.Industry(x, z) * Mathf.Lerp(.35f, 1, spread);
            path = bare * zones.Kampung(x, z) * .85f;
        }
    }
}
