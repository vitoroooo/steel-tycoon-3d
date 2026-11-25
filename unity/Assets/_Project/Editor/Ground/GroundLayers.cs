using UnityEditor;
using UnityEngine;

namespace Steel.Editor.Ground
{
    /// <summary>Terrain layers built from the Poly Haven ground textures, in the order GroundPaint blends them.</summary>
    internal static class GroundLayers
    {
        internal static TerrainLayer[] Create() => new[]
        {
            Layer("brown_mud_02", 6, new Color(.62f, .58f, .52f)),
            Layer("leafy_grass", 4.5f, new Color(.78f, .86f, .62f)),
            Layer("gravel_floor_02", 3, new Color(.66f, .64f, .62f)),
            Layer("grass_path_2", 4, new Color(.8f, .76f, .68f)),
        };

        /// <param name="metres">World size of one texture repeat.</param>
        private static TerrainLayer Layer(string id, float metres, Color tint)
        {
            string textures = "Assets/ThirdParty/PolyHaven/" + id + "/", path = CityMaterials.Generated + "Tanah " + id + ".terrainlayer";
            var normal = (TextureImporter)AssetImporter.GetAtPath(textures + "normal.jpg");
            if (normal.textureType != TextureImporterType.NormalMap)
            { normal.textureType = TextureImporterType.NormalMap; normal.SaveAndReimport(); }
            var layer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(path);
            if (!layer) { layer = new TerrainLayer(); AssetDatabase.CreateAsset(layer, path); }
            layer.diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(textures + "color.jpg");
            layer.normalMapTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(textures + "normal.jpg");
            layer.tileSize = Vector2.one * metres;
            layer.diffuseRemapMax = tint;       // multiplies the colour map
            layer.normalScale = 1.1f;
            layer.metallic = 0; layer.smoothness = .12f;   // no mask map: its empty AO channel would blacken the ground
            EditorUtility.SetDirty(layer);
            return layer;
        }
    }
}
