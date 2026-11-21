using UnityEditor;
using UnityEngine;

namespace Steel.Editor.Characters
{
    /// <summary>Builds URP materials for the three Rocketbox material slots: body, head and hair.</summary>
    internal static class CharacterMaterials
    {
        internal static Material For(CharacterSpec spec, string sourceName)
        {
            if (sourceName.EndsWith("_opacity")) return Hair(spec);
            return Opaque(spec, sourceName.EndsWith("_head") ? "head" : "body");
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
