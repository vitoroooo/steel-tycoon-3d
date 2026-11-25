using System.IO;
using UnityEditor;

namespace Steel.Editor.Characters
{
    /// <summary>Import settings for Microsoft Rocketbox avatars and their animation files.</summary>
    internal sealed class RocketboxImportRules : AssetPostprocessor
    {
        internal const string Root = "Assets/ThirdParty/Rocketbox/";
        /// <summary>Top bone of every Rocketbox skeleton; the clips store their travel on it.</summary>
        internal const string RootBone = "Bip01";
        private bool Applies => assetPath.StartsWith(Root);

        private void OnPreprocessTexture()
        {
            if (!Applies) return;
            var importer = (TextureImporter)assetImporter;
            string name = Path.GetFileNameWithoutExtension(assetPath);
            if (name.EndsWith("_normal")) importer.textureType = TextureImporterType.NormalMap;
            if (name.EndsWith("_specular")) importer.sRGBTexture = false;
            if (name.EndsWith("_opacity_color")) importer.alphaIsTransparency = true;
            importer.maxTextureSize = 2048;
        }

        private void OnPreprocessModel()
        {
            if (!Applies) return;
            var importer = (ModelImporter)assetImporter;
            importer.animationType = ModelImporterAnimationType.Generic;
            importer.importBlendShapes = false;
            importer.importAnimation = assetPath.Contains("/Animations/");
        }

        private void OnPreprocessAnimation()
        {
            if (!Applies) return;
            var importer = (ModelImporter)assetImporter;
            var clips = importer.defaultClipAnimations;
            foreach (var clip in clips)
            {
                clip.name = Path.GetFileNameWithoutExtension(assetPath);
                clip.loopTime = true;
            }
            importer.clipAnimations = clips;
        }
    }
}
