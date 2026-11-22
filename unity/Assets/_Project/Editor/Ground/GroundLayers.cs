using UnityEditor;
using UnityEngine;
using Steel.Editor.Toon;

namespace Steel.Editor.Ground
{
    /// <summary>Terrain layers painted in flat cartoon tones, in the order GroundPaint blends them.</summary>
    internal static class GroundLayers
    {
        internal static TerrainLayer[] Create() => new[]
        {
            Layer("brown_mud_02", 7, "D9C7A3", "CDB88F", .38f),      // bare earth
            Layer("leafy_grass", 6, "8FBF55", "7DB048", .42f),       // grass
            Layer("gravel_floor_02", 4, "BEB7A6", "ABA493", .34f),   // gravel
            Layer("grass_path_2", 5, "E6D9B8", "DACBA5", .36f),      // trodden path
        };

        /// <param name="id">The layer's old texture name: asset paths and GroundPaint's order still use it.</param>
        /// <param name="metres">World size of one texture repeat.</param>
        private static TerrainLayer Layer(string id, float metres, string main, string patch, float share)
        {
            string path = CityMaterials.Generated + "Tanah " + id + ".terrainlayer";
            var layer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(path);
            if (!layer) { layer = new TerrainLayer(); AssetDatabase.CreateAsset(layer, path); }
            layer.diffuseTexture = GroundSwatch.Create(id, ToonPalette.Hex(main), ToonPalette.Hex(patch), share, metres);
            layer.normalMapTexture = null; layer.maskMapTexture = null;
            layer.tileSize = Vector2.one * metres;
            layer.diffuseRemapMax = Color.white;
            layer.metallic = 0; layer.smoothness = 0;
            EditorUtility.SetDirty(layer);
            return layer;
        }
    }
}
