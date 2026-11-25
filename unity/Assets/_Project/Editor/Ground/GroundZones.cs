using UnityEngine;
using Steel.Services;

namespace Steel.Editor.Ground
{
    /// <summary>Answers "what kind of ground is here?" for any world position; the one place that knows the districts.</summary>
    internal sealed class GroundZones
    {
        internal const float Size = 480, Depth = 40, Base = 4;   // terrain square, height range, flat level above its floor
        internal static readonly Vector3 Centre = new Vector3(0, 0, 5);
        /// <summary>World height of the flat city ground; streets and yards sit on top of it.</summary>
        internal const float CityLevel = -.1f;

        private readonly float halfWidth, minZ, maxZ;

        internal GroundZones(CityLayout layout)
        {
            halfWidth = layout.BoundaryHalfWidth; minZ = layout.BoundaryMinZ; maxZ = layout.BoundaryMaxZ;
        }

        /// <summary>Metres beyond the playable boundary; zero anywhere inside it.</summary>
        internal float Outside(float x, float z) =>
            Mathf.Max(0, Mathf.Max(Mathf.Abs(x) - halfWidth, Mathf.Max(minZ - z, z - maxZ)));

        /// <summary>Broad wandering patches (0..1) that decide where weeds and gravel gather.</summary>
        internal static float Patches(float x, float z, float seed) =>
            Mathf.PerlinNoise(x * .043f + seed, z * .043f + seed) * .7f + Mathf.PerlinNoise(x * .19f - seed, z * .19f + seed) * .3f;

        /// <summary>The kampung on the west side: footpaths and gardens rather than churned mud.</summary>
        internal static float Kampung(float x, float z) => Mathf.InverseLerp(-33, -37, x) * Mathf.InverseLerp(-12, -7, z);

        /// <summary>Workshops and factories east of the main road, and the smelter grounds to the south-east.</summary>
        internal static float Industry(float x, float z) =>
            Mathf.Max(Mathf.InverseLerp(11, 15, x) * Mathf.InverseLerp(-9, -5, z),
                      Mathf.InverseLerp(-2, 4, x) * Mathf.InverseLerp(-17, -21, z));

        /// <summary>How much wild grass grows here (0..1): everywhere outside, in patches inside.</summary>
        internal float Grass(float x, float z)
        {
            float wild = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.5f, .68f, Patches(x, z, 11.3f)));
            float trampled = Mathf.Lerp(1, .3f, Industry(x, z));
            return Mathf.Clamp01(wild * trampled + Mathf.InverseLerp(0, 9, Outside(x, z)));
        }
    }
}
