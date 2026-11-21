using UnityEditor;
using UnityEngine;

namespace Steel.Editor.Characters
{
    /// <summary>Builds URP materials for the Rocketbox slots: body, head, hair, and a worker's helmet and tools.</summary>
    internal static class CharacterMaterials
    {
        internal static Material For(CharacterSpec spec, string sourceName)
        {
            string part = sourceName.Substring(sourceName.LastIndexOf('_') + 1);   // "m017_head" → "head"
            return part == "opacity" ? Hair(spec) : Opaque(spec, part);
        }

        private static Material Opaque(CharacterSpec spec, string part)
        {
            var material = CityMaterials.Plain(spec.Folder + " " + part, Color.white, 0, .22f);
            material.SetTexture("_BaseMap", Load(spec.TexturePath(part, "color", ".jpg")));
            material.SetTexture("_BumpMap", Load(spec.TexturePath(part, "normal", ".png")));
            material.EnableKeyword("_NORMALMAP");
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material Hair(CharacterSpec spec)
        {
            var material = CityMaterials.Plain(spec.Folder + " hair", Color.white, 0, .15f);
            material.SetTexture("_BaseMap", Load(spec.TexturePath("opacity", "color", ".png")));
            material.SetFloat("_AlphaClip", 1);
            material.SetFloat("_Cutoff", .4f);
            material.SetFloat("_Cull", 0);
            material.EnableKeyword("_ALPHATEST_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Texture2D Load(string path)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture == null) throw new System.InvalidOperationException("Texture not imported: " + path);
            return texture;
        }
    }
}
