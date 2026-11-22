using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Steel.Editor
{
    /// <summary>The render pipeline of the editor and desktop build: the same flat look the phone gets, a little sharper.</summary>
    internal static class CityLighting
    {
        internal static void Create()
        {
            string path = CityMaterials.Generated;
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(path + "ToonRenderer.asset");
            if (!renderer)
            {
                renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, path + "ToonRenderer.asset");
            }
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path + "ToonPipeline.asset");
            if (!pipeline)
            {
                pipeline = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(pipeline, path + "ToonPipeline.asset");
            }
            // No HDR, no post-processing, no depth texture: the palette is the picture.
            pipeline.supportsCameraDepthTexture = false; pipeline.supportsHDR = false;
            pipeline.msaaSampleCount = 4; pipeline.shadowDistance = 70; pipeline.useSRPBatcher = true;
            var settings = new SerializedObject(pipeline);
            settings.FindProperty("m_MainLightShadowsSupported").boolValue = true;
            settings.FindProperty("m_MainLightShadowmapResolution").intValue = 2048;
            settings.FindProperty("m_SoftShadowsSupported").boolValue = false;
            settings.FindProperty("m_AdditionalLightsRenderingMode").intValue = (int)LightRenderingMode.Disabled;
            settings.ApplyModifiedPropertiesWithoutUndo();
            GraphicsSettings.renderPipelineAsset = pipeline; QualitySettings.renderPipeline = pipeline;
            Atmosphere.SkyDome.Create();
            EditorUtility.SetDirty(renderer); EditorUtility.SetDirty(pipeline);
        }
    }
}
