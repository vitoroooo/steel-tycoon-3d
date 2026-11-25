using UnityEditor;
using UnityEngine;
using Steel.Editor.Toon;

namespace Steel.Editor
{
    /// <summary>Every material in the city: flat toon colours from one palette, no textures.</summary>
    internal static class CityMaterials
    {
        internal const string Generated = "Assets/_Project/Generated/";
        private const string Flat = "Steel/Toon", Lined = "Steel/ToonOutlined";

        /// <summary>A named surface from the palette. Tiling is ignored: flat colour has no texture to repeat.</summary>
        internal static Material Surface(string id, Vector2 tiling, string name = null, Color? tint = null) =>
            Plain(name ?? id, ToonPalette.Surface(id, tint));

        /// <summary>A flat-colour material. Metal and smoothness are accepted for old callers and ignored.</summary>
        internal static Material Plain(string name, Color color, float metal = 0, float smooth = .4f) =>
            Paint(name, color, Flat);

        /// <summary>Flat colour with a drawn outline: for characters, vehicles and rounded props.</summary>
        internal static Material Outlined(string name, Color color, float width = .012f)
        {
            Material mat = Paint(name, color, Lined);
            mat.SetFloat("_OutlineWidth", width); mat.SetColor("_OutlineColor", ToonPalette.Ink);
            return mat;
        }

        /// <summary>A textured URP material, only for what flat colour cannot draw: cut-out leaves and ground decals.</summary>
        internal static Material Textured(string name, Color color, float smooth = .4f)
        {
            Material mat = Paint(name, color, "Universal Render Pipeline/Lit");
            mat.SetFloat("_Smoothness", smooth); return mat;
        }

        internal static Material Glow(string name, Color color)
        {
            Material mat = Plain(name, color);
            mat.SetColor("_EmissionColor", color * .6f);
            return mat;
        }

        private static Material Paint(string name, Color color, string shaderName)
        {
            string path = Generated + name + ".mat";
            Shader shader = Shader.Find(shaderName);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (!mat) { mat = new Material(shader); AssetDatabase.CreateAsset(mat, path); }
            else if (mat.shader != shader)
            {
                // Rebuild in place: stale texture slots from the old PBR material would still ship in the APK.
                var clean = new Material(shader) { name = mat.name };
                EditorUtility.CopySerialized(clean, mat); Object.DestroyImmediate(clean);
            }
            mat.SetColor("_BaseColor", color); mat.SetColor("_EmissionColor", Color.black);
            EditorUtility.SetDirty(mat); return mat;
        }
    }
}
