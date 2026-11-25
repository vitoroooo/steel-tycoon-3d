using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Steel.Editor
{
    internal static class CityLighting
    {
        internal static void Create()
        {
            string path = CityMaterials.Generated;
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(path + "CityRenderer.asset");
            if (!renderer)
            {
                renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, path + "CityRenderer.asset");
                var type = typeof(UniversalRendererData).Assembly.GetType(
                    "UnityEngine.Rendering.Universal.ScreenSpaceAmbientOcclusion");
                if (type != null)
                {
                    var ao = (ScriptableRendererFeature)ScriptableObject.CreateInstance(type);
                    ao.name = "Screen Space Ambient Occlusion"; AssetDatabase.AddObjectToAsset(ao, renderer);
                    var settings = new SerializedObject(ao);
                    settings.FindProperty("m_Settings.Downsample").boolValue = true;
                    settings.FindProperty("m_Settings.Intensity").floatValue = 1.3f;
                    settings.FindProperty("m_Settings.Radius").floatValue = .18f;
                    settings.ApplyModifiedPropertiesWithoutUndo(); renderer.rendererFeatures.Add(ao);
                }
            }
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path + "CityPipeline.asset");
            if (!pipeline)
            {
                pipeline = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(pipeline, path + "CityPipeline.asset");
            }
            pipeline.supportsCameraDepthTexture = true; pipeline.supportsHDR = true;
            pipeline.msaaSampleCount = 2; pipeline.shadowDistance = 95;
            var pipelineSettings = new SerializedObject(pipeline);
            pipelineSettings.FindProperty("m_MainLightShadowsSupported").boolValue = true;
            pipelineSettings.FindProperty("m_MainLightShadowmapResolution").intValue = 2048;
            pipelineSettings.FindProperty("m_SoftShadowsSupported").boolValue = true;
            pipelineSettings.ApplyModifiedPropertiesWithoutUndo();
            GraphicsSettings.renderPipelineAsset = pipeline; QualitySettings.renderPipeline = pipeline;
            var sun = new GameObject("Sore / Matahari").AddComponent<Light>();
            sun.type = LightType.Directional; sun.color = new Color(1, .84f, .66f);
            sun.intensity = 1.7f; sun.shadows = LightShadows.Soft; sun.shadowStrength = .85f;
            sun.transform.rotation = Quaternion.Euler(38, -35, 0); RenderSettings.sun = sun;
            var sky = AssetDatabase.LoadAssetAtPath<Material>(path + "Langit.mat");
            if (!sky) { sky = new Material(Shader.Find("Skybox/Panoramic")); AssetDatabase.CreateAsset(sky, path + "Langit.mat"); }
            sky.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture>("Assets/ThirdParty/PolyHaven/sky.hdr"));
            sky.SetFloat("_Exposure", .85f); sky.SetFloat("_Rotation", 125);
            RenderSettings.skybox = sky; RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(.34f, .44f, .58f);
            RenderSettings.ambientEquatorColor = new Color(.25f, .28f, .32f);
            RenderSettings.ambientGroundColor = new Color(.12f, .12f, .12f);
            RenderSettings.fog = true; RenderSettings.fogColor = new Color(.65f, .66f, .66f);
            RenderSettings.fogMode = FogMode.Exponential; RenderSettings.fogDensity = .004f;
            var volume = new GameObject("Atmosfer / ACES").AddComponent<Volume>(); volume.isGlobal = true;
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path + "CityVolume.asset");
            if (!profile)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, path + "CityVolume.asset");
                var tone = profile.Add<Tonemapping>(); tone.mode.Override(TonemappingMode.ACES);
                var bloom = profile.Add<Bloom>(); bloom.intensity.Override(.24f); bloom.threshold.Override(1.1f);
                var color = profile.Add<ColorAdjustments>(); color.contrast.Override(14); color.postExposure.Override(.2f);
                foreach (var component in profile.components) AssetDatabase.AddObjectToAsset(component, profile);
            }
            volume.sharedProfile = profile;
            if (profile.TryGet<ColorAdjustments>(out var adjustments)) adjustments.postExposure.Override(-.2f);
            EditorUtility.SetDirty(profile);
            EditorUtility.SetDirty(renderer); EditorUtility.SetDirty(pipeline); EditorUtility.SetDirty(sky);
        }
    }
}
