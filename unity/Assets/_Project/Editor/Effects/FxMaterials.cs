using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Steel.Editor.Effects
{
    /// <summary>Particle materials built from the self-made sprites in Textures/Fx.</summary>
    internal static class FxMaterials
    {
        /// <summary>additive is for things that give off light (sparks); smoke blends normally.</summary>
        internal static Material Sprite(string name, string texture, bool additive)
        {
            string path = CityMaterials.Generated + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (!material)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
                AssetDatabase.CreateAsset(material, path);
            }
            material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Textures/Fx/" + texture + ".png"));
            material.SetColor("_BaseColor", additive ? new Color(3, 2.2f, 1.2f) : Color.white);
            material.SetFloat("_Surface", 1);
            material.SetFloat("_Blend", additive ? 2 : 0);
            material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)(additive ? BlendMode.One : BlendMode.OneMinusSrcAlpha));
            material.SetFloat("_ZWrite", 0);
            material.SetFloat("_ColorMode", 0);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.SetOverrideTag("RenderType", "Transparent");
            material.renderQueue = (int)RenderQueue.Transparent;
            EditorUtility.SetDirty(material);
            return material;
        }
    }
}
