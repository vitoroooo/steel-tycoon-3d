using UnityEngine;
using UnityEngine.Rendering;

namespace Steel.Editor.Ground
{
    /// <summary>Switches a URP Lit material to alpha blending; the shader needs every one of these set together.</summary>
    internal static class Transparency
    {
        internal static void Apply(Material material)
        {
            material.SetFloat("_Surface", 1);
            material.SetFloat("_Blend", 0);
            material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_ZWrite", 0);
            // Otherwise URP keeps the full sky reflection even where the texture is fully transparent.
            material.SetFloat("_BlendModePreserveSpecular", 0);
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.SetOverrideTag("RenderType", "Transparent");
            material.renderQueue = (int)RenderQueue.Transparent;
        }
    }
}
