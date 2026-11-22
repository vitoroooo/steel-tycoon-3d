using UnityEditor;
using UnityEngine;
using Steel.Services;

namespace Steel.Editor.Ground
{
    /// <summary>Scatters rain puddles over bare ground and oil stains over industrial yards. Call once the city stands.</summary>
    internal static class GroundDecals
    {
        private const int Puddles = 46, Stains = 34, Seed = 1977, MaskCells = 1024;
        private const float Lift = .03f;   // above the terrain, below every street and yard slab

        internal static void Create(CityLayout layout)
        {
            var zones = new GroundZones(layout);
            var built = new GroundMask(MaskCells);   // taken before any decal exists, so decals may overlap each other
            var random = new System.Random(Seed);
            var puddle = Material("Genangan", "puddle", new Color(.62f, .68f, .7f, .78f), .97f);
            var stain = Material("Noda oli", "oil_stain", new Color(1, 1, 1, .85f), .72f);
            Scatter("Genangan", puddle, Puddles, 1.6f, 4.2f, zones, built, random, layout, (x, z) => 1 - GroundZones.Industry(x, z) * .6f);
            Scatter("Noda oli", stain, Stains, .7f, 2.1f, zones, built, random, layout, GroundZones.Industry);
        }

        /// <param name="chance">Likelihood (0..1) that a spot at this position is kept.</param>
        private static void Scatter(string name, Material material, int count, float small, float large,
            GroundZones zones, GroundMask built, System.Random random, CityLayout layout, System.Func<float, float, float> chance)
        {
            for (int placed = 0, tries = 0; placed < count && tries < count * 40; tries++)
            {
                float x = Mathf.Lerp(-layout.BoundaryHalfWidth, layout.BoundaryHalfWidth, (float)random.NextDouble());
                float z = Mathf.Lerp(layout.BoundaryMinZ, layout.BoundaryMaxZ, (float)random.NextDouble());
                if (zones.Grass(x, z) > .25f || random.NextDouble() > chance(x, z)) continue;
                float size = Mathf.Lerp(small, large, (float)random.NextDouble());
                if (built.Covers(x, z, size / 2)) continue;   // never under a building or across a street
                var quad = CityGeometry.Shape(name, PrimitiveType.Quad, new Vector3(x, GroundZones.CityLevel + Lift, z),
                    new Vector3(size, size * Mathf.Lerp(.6f, 1, (float)random.NextDouble()), 1), material, false);
                quad.transform.rotation = Quaternion.Euler(90, (float)random.NextDouble() * 360, 0);
                quad.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                placed++;
            }
        }

        private static Material Material(string name, string texture, Color tint, float smoothness)
        {
            var material = CityMaterials.Textured(name, tint, smoothness);
            material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Textures/Ground/" + texture + ".png"));
            Transparency.Apply(material);
            EditorUtility.SetDirty(material);
            return material;
        }
    }
}
