using UnityEditor;
using UnityEngine;

namespace Steel.Editor.Buildings
{
    /// <summary>Materials from the self-painted textures in Textures/Nature (see tools/assets/make_foliage_textures.py).</summary>
    internal static class FoliageMaterial
    {
        private const string Folder = "Assets/_Project/Textures/Nature/";
        private const float Cutoff = .5f;

        /// <summary>A leaf whose outline is cut by the colour texture's alpha.</summary>
        internal static Material Cutout(string name, string colour, string normal)
        {
            var mat = Opaque(name, colour, Vector2.one, .3f);
            var importer = (TextureImporter)AssetImporter.GetAtPath(Folder + normal + ".png");
            if (importer.textureType != TextureImporterType.NormalMap)
            { importer.textureType = TextureImporterType.NormalMap; importer.SaveAndReimport(); }
            mat.SetTexture("_BumpMap", AssetDatabase.LoadAssetAtPath<Texture2D>(Folder + normal + ".png"));
            mat.EnableKeyword("_NORMALMAP");
            mat.SetFloat("_AlphaClip", 1); mat.SetFloat("_Cutoff", Cutoff); mat.EnableKeyword("_ALPHATEST_ON");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            return mat;
        }

        internal static Material Opaque(string name, string colour, Vector2 tiling, float smooth)
        {
            var mat = CityMaterials.Textured(name, Color.white, smooth);
            mat.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(Folder + colour + ".png"));
            mat.SetTextureScale("_BaseMap", tiling);
            EditorUtility.SetDirty(mat);
            return mat;
        }
    }
}
