using System.IO;
using UnityEditor;
using UnityEngine;

namespace Steel.Editor.Ground
{
    /// <summary>Paints a small tileable ground texture: one flat colour with hard-edged patches of a second tone.</summary>
    internal static class GroundSwatch
    {
        private const int Size = 128;
        private const string Folder = "Assets/_Project/Textures/Ground/Kartun/";

        /// <param name="share">Fraction of the tile the second tone covers.</param>
        internal static Texture2D Create(string name, Color main, Color patch, float share, float seed)
        {
            string path = Folder + name + ".png";
            var texture = new Texture2D(Size, Size, TextureFormat.RGB24, false);
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                    texture.SetPixel(x, y, Blotch(x / (float)Size, y / (float)Size, seed) < share ? patch : main);
            Directory.CreateDirectory(Folder);
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer.wrapMode != TextureWrapMode.Repeat || importer.maxTextureSize != Size)
            {
                importer.wrapMode = TextureWrapMode.Repeat; importer.maxTextureSize = Size;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        /// <summary>Noise in 0..1 that wraps at the tile edge: Perlin sampled round two circles.</summary>
        private static float Blotch(float u, float v, float seed)
        {
            const float Turn = Mathf.PI * 2, Radius = .55f;
            float a = Mathf.PerlinNoise(seed + Mathf.Cos(u * Turn) * Radius, seed + Mathf.Sin(u * Turn) * Radius);
            float b = Mathf.PerlinNoise(seed + 9 + Mathf.Cos(v * Turn) * Radius, seed + 9 + Mathf.Sin(v * Turn) * Radius);
            float c = Mathf.PerlinNoise(seed + 17 + Mathf.Cos((u + v) * Turn) * Radius * 2,
                seed + 17 + Mathf.Sin((u - v) * Turn) * Radius * 2);
            return (a + b + c) / 3;
        }
    }
}
