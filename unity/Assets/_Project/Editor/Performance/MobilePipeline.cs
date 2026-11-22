using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Steel.Editor.Performance
{
    public static class MobilePipeline
    {
        internal static UniversalRenderPipelineAsset Prepare()
        {
            string root = CityMaterials.Generated;
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(root + "MobileRenderer.asset");
            if (!renderer)
            {
                renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, root + "MobileRenderer.asset");
            }
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(root + "MobilePipeline.asset");
            if (!pipeline)
            {
                pipeline = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(pipeline, root + "MobilePipeline.asset");
            }
            pipeline.supportsCameraDepthTexture = false; pipeline.supportsCameraOpaqueTexture = false;
            pipeline.supportsHDR = false; pipeline.msaaSampleCount = 2;   // cheap on tile GPUs, and outlines need it
            pipeline.renderScale = .80f;
            pipeline.shadowDistance = 28; pipeline.shadowCascadeCount = 1; pipeline.useSRPBatcher = true;
            var so = new SerializedObject(pipeline);
            so.FindProperty("m_MainLightShadowmapResolution").intValue = 1024;
            so.FindProperty("m_SoftShadowsSupported").boolValue = false;
            so.FindProperty("m_AdditionalLightsRenderingMode").intValue = (int)LightRenderingMode.Disabled;
            so.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(pipeline); AssetDatabase.SaveAssets();
            return pipeline;
        }
        public static void Install()
        {
            var pipeline = Prepare();
            var config = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0]);
            int index = Array.IndexOf(QualitySettings.names, "Medium");
            if (index < 0) throw new InvalidOperationException("Medium (Android) quality preset is missing.");
            config.FindProperty("m_QualitySettings").GetArrayElementAtIndex(index)
                .FindPropertyRelative("customRenderPipeline").objectReferenceValue = pipeline;
            config.ApplyModifiedPropertiesWithoutUndo(); AssetDatabase.SaveAssets();
        }
    }
}
