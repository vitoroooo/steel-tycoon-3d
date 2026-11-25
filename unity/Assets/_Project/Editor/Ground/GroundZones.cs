using UnityEngine;
using Steel.Services;

namespace Steel.Editor.Ground
{
    /// <summary>Answers "what kind of ground is here?" for any world position; the one place that knows the districts.</summary>
    internal sealed class GroundZones
    {
        internal const float Size = 800, Depth = 40, Base = 4;   // terrain square, height range, flat level above its floor
        internal static readonly Vector3 Centre = Vector3.zero;
        /// <summary>World height of the flat city ground; streets and yards sit on top of it.</summary>
        internal const float CityLevel = -.1f;

        internal readonly GroundDistricts Districts = new GroundDistricts();
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

        /// <summary>Kampung ground: footpaths and gardens rather than churned mud.</summary>
        internal float Kampung(float x, float z) => Districts.Core(x, z)
            ? Mathf.InverseLerp(-33, -37, x) * Mathf.InverseLerp(-12, -7, z)
            : Share(x, z, "kampung", 1, "pusat", .9f, "pasar", .9f);

        /// <summary>Workshops, factories and scrap yards: gravel worn thin over mud.</summary>
        internal float Industry(float x, float z) => Districts.Core(x, z)
            ? Mathf.Max(Mathf.InverseLerp(11, 15, x) * Mathf.InverseLerp(-9, -5, z),
                        Mathf.InverseLerp(-2, 4, x) * Mathf.InverseLerp(-17, -21, z))
            : Share(x, z, "industri", 1, "selatan", .7f, null, 0);

        private float Share(float x, float z, string first, float firstShare, string second, float secondShare,
            string third, float thirdShare)
        {
            string area = Districts.Area(x, z);
            return area == first ? firstShare : area == second ? secondShare : area == third ? thirdShare : 0;
        }

        /// <summary>How much wild grass grows here (0..1): everywhere outside, in patches inside.</summary>
        internal float Grass(float x, float z)
        {
            float wild = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.5f, .68f, Patches(x, z, 11.3f)));
            float trampled = Mathf.Lerp(1, .3f, Industry(x, z));
            if (!Districts.Core(x, z)) wild = Districts.Area(x, z) == "pinggir" ? 1 : wild * .5f;
            return Mathf.Clamp01(wild * trampled + Mathf.InverseLerp(0, 9, Outside(x, z)));
        }
    }
}
