using UnityEditor;
using UnityEngine;

namespace Steel.Editor
{
    internal static class CityMaterials
    {
        internal const string Generated = "Assets/_Project/Generated/";
        /// <summary>A PBR surface; pass a name to get a separate material with its own tiling.</summary>
        internal static Material Surface(string id, Vector2 tiling, string name = null, Color? tint = null)
        {
            string path = "Assets/ThirdParty/PolyHaven/" + id + "/";
            var normal = (TextureImporter)AssetImporter.GetAtPath(path + "normal.jpg");
            if (normal.textureType != TextureImporterType.NormalMap)
            { normal.textureType = TextureImporterType.NormalMap; normal.SaveAndReimport(); }
            var mask = (TextureImporter)AssetImporter.GetAtPath(path + "mask.png");
            if (mask.sRGBTexture) { mask.sRGBTexture = false; mask.SaveAndReimport(); }
            var mat = Plain(name ?? id, Color.white, 0, .5f);
            mat.SetColor("_BaseColor", tint.HasValue ? tint.Value : id == "asphalt_02" ? new Color(.4f, .44f, .48f) :
                id == "brown_mud_02" ? new Color(.6f, .58f, .52f) :
                id == "concrete_floor_02" ? new Color(.75f, .78f, .8f) : Color.white);
            mat.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(path + "color.jpg"));
            mat.SetTexture("_BumpMap", AssetDatabase.LoadAssetAtPath<Texture2D>(path + "normal.jpg"));
            mat.SetTexture("_MetallicGlossMap", AssetDatabase.LoadAssetAtPath<Texture2D>(path + "mask.png"));
            mat.EnableKeyword("_NORMALMAP"); mat.EnableKeyword("_METALLICSPECGLOSSMAP");
            mat.SetFloat("_BumpScale", 1.1f); mat.SetTextureScale("_BaseMap", tiling);
            EditorUtility.SetDirty(mat); return mat;
        }
        internal static Material Plain(string name, Color color, float metal = 0, float smooth = .4f)
        {
            string path = Generated + name + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (!mat)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.SetColor("_BaseColor", color); mat.SetFloat("_Metallic", metal); mat.SetFloat("_Smoothness", smooth);
            EditorUtility.SetDirty(mat); return mat;
        }
        internal static Material Glow(string name, Color color)
        {
            var mat = Plain(name, color, .1f, .65f);
            mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor", color * 2);
            return mat;
        }
    }
}
