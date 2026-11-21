using UnityEditor;
using UnityEngine;

namespace Steel.Editor
{
    /// <summary>The font and material every painted sign shares, so lettering is occluded like any surface.</summary>
    internal static class SignPaint
    {
        private const string Path = CityMaterials.Generated + "Cat papan.mat";

        internal static Font Font => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        internal static Material Material()
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(Path);
            if (!mat)
            {
                mat = new Material(Shader.Find("Steel/SignText"));
                AssetDatabase.CreateAsset(mat, Path);
            }
            mat.mainTexture = Font.material.mainTexture;
            EditorUtility.SetDirty(mat);
            return mat;
        }
    }
}
