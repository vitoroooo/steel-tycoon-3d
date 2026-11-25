using UnityEditor;
using UnityEngine;
using Steel.Services;

namespace Steel.Editor.Ground
{
    /// <summary>Plants tufts of wild grass wherever the grass layer is painted, and sets the wind that sways them.</summary>
    internal static class GroundCover
    {
        internal const float ViewDistance = 70;
        private const int Cells = 1024, PerPatch = 16, Thickest = 3;
        private const float Reach = 24;   // metres past the boundary that still get tufts; further out only the texture shows

        /// <summary>Call after everything else is built: grass stays off whatever is paved or built over.</summary>
        internal static void Plant(Terrain terrain, CityLayout layout)
        {
            var data = terrain.terrainData;
            var zones = new GroundZones(layout);
            var built = new GroundMask(Cells);
            data.SetDetailResolution(Cells, PerPatch);
            data.SetDetailScatterMode(DetailScatterMode.InstanceCountMode);
            data.detailPrototypes = new[]
            {
                Tuft("grass_tuft", .45f, .8f, new Color(.5f, .72f, .3f), new Color(.42f, .62f, .26f)),
                Tuft("grass_tuft_dry", .35f, .6f, new Color(.72f, .72f, .36f), new Color(.62f, .6f, .3f)),
            };
            data.wavingGrassStrength = .45f; data.wavingGrassSpeed = .35f; data.wavingGrassAmount = .28f;
            data.wavingGrassTint = new Color(.93f, .9f, .78f);
            var green = new int[Cells, Cells];
            var dry = new int[Cells, Cells];
            for (int row = 0; row < Cells; row++)
                for (int column = 0; column < Cells; column++)
                {
                    float x = GroundZones.Centre.x + ((column + .5f) / Cells - .5f) * GroundZones.Size;
                    float z = GroundZones.Centre.z + ((row + .5f) / Cells - .5f) * GroundZones.Size;
                    if (zones.Outside(x, z) > Reach || built.Covers(row, column)) continue;
                    float grass = zones.Grass(x, z);
                    if (grass < .5f) continue;
                    // Dry tufts take over where the patch noise runs low, so colour varies across a field.
                    float parched = Mathf.InverseLerp(.5f, .3f, GroundZones.Patches(x, z, 71.9f));
                    green[row, column] = Mathf.RoundToInt(grass * Thickest * (1 - parched));
                    dry[row, column] = Mathf.RoundToInt(grass * Thickest * parched * .7f);
                }
            data.SetDetailLayer(0, 0, 0, green);
            data.SetDetailLayer(0, 0, 1, dry);
            EditorUtility.SetDirty(data);
        }

        private static DetailPrototype Tuft(string texture, float minHeight, float maxHeight, Color healthy, Color dry) =>
            new DetailPrototype
            {
                prototypeTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Textures/Ground/" + texture + ".png"),
                renderMode = DetailRenderMode.Grass, usePrototypeMesh = false,
                minWidth = .5f, maxWidth = .9f, minHeight = minHeight, maxHeight = maxHeight,
                healthyColor = healthy, dryColor = dry, noiseSpread = .3f,
            };
    }
}
