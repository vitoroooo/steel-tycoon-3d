using UnityEditor;
using UnityEngine;

namespace Steel.Editor.Plan.Town
{
    /// <summary>The whole generated town in two strips and one material, so a phone draws it in a few batches per sector.</summary>
    internal sealed class TownKit
    {
        private const int Swatch = 4;
        private const string MapPath = CityMaterials.Generated + "Palet kota.asset";
        /// <summary>Walls and anything else that stops a player or a truck.</summary>
        internal readonly Strip Solid = new Strip();
        /// <summary>Roofs, windows, signs and ground patches: seen, never bumped into.</summary>
        internal readonly Strip Trim = new Strip();

        internal void Build()
        {
            Material material = CityMaterials.Plain("Kota", Color.white);
            material.SetTexture("_BaseMap", PaletteMap());
            Solid.Build("Kota padat", material, true, true);
            Trim.Build("Kota hias", material, false);
        }

        // Point-sampled swatches: every quad reads the middle of one, so colours never bleed.
        private static Texture2D PaletteMap()
        {
            var map = new Texture2D(Tones.Columns * Swatch, Tones.Rows * Swatch, TextureFormat.RGB24, false)
            {
                name = "Palet kota", filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp
            };
            for (int x = 0; x < map.width; x++)
                for (int y = 0; y < map.height; y++)
                    map.SetPixel(x, y, Tones.Colour((Tone)(y / Swatch * Tones.Columns + x / Swatch)));
            map.Apply();
            AssetDatabase.DeleteAsset(MapPath);
            AssetDatabase.CreateAsset(map, MapPath);
            return map;
        }
    }
}
