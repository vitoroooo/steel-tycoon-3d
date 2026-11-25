using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Steel.Presentation.World;

namespace Steel.Editor.Atmosphere
{
    /// <summary>The sky picture, the sun lined up with it, ambient light, haze and drifting cloud shadows.</summary>
    internal static class SkyDome
    {
        // Where the sun sits in sky.hdr (printed by tools/assets/download_polyhaven_sky.py).
        private const float ImageSunYaw = 34.2f, SunPitch = 47.8f;
        /// <summary>Compass bearing of the sun in the world; shadows fall away from it.</summary>
        private const float SunBearing = 145;
        private const float Exposure = .85f, SunIntensity = 1.9f;
        private const float ShadowTile = 90;   // metres of ground one cloud-shadow tile covers

        internal static Light Create()
        {
            var sun = new GameObject("Langit / Matahari").AddComponent<Light>();
            sun.type = LightType.Directional; sun.color = new Color(1, .95f, .86f);
            sun.intensity = SunIntensity; sun.shadows = LightShadows.Soft; sun.shadowStrength = .85f;
            sun.transform.rotation = Quaternion.Euler(SunPitch, SunBearing + 180, 0);
            RenderSettings.sun = sun;
            RenderSettings.skybox = Sky();
            Ambient();
            sun.cookie = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Textures/Sky/cloud_shadow.png");
            sun.gameObject.AddComponent<UniversalAdditionalLightData>().lightCookieSize = Vector2.one * ShadowTile;
            sun.gameObject.AddComponent<CloudShadows>();
            sun.gameObject.AddComponent<SkyDrift>();
            return sun;
        }

        private static Material Sky()
        {
            string path = CityMaterials.Generated + "Langit.mat";
            var sky = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (!sky) { sky = new Material(Shader.Find("Skybox/Panoramic")); AssetDatabase.CreateAsset(sky, path); }
            sky.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture>("Assets/ThirdParty/PolyHaven/sky.hdr"));
            sky.SetFloat("_Exposure", Exposure);
            // Skybox/Panoramic shows the image centre due east with image-right towards the south, and
            // _Rotation turns the dome anticlockwise: the painted sun lands on bearing 90 + ImageSunYaw - rotation.
            sky.SetFloat("_Rotation", Mathf.Repeat(90 + ImageSunYaw - SunBearing, 360));
            EditorUtility.SetDirty(sky);
            return sky;
        }

        private static void Ambient()
        {
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(.5f, .6f, .76f);
            RenderSettings.ambientEquatorColor = new Color(.46f, .48f, .5f);
            RenderSettings.ambientGroundColor = new Color(.19f, .18f, .16f);
            RenderSettings.fog = true; RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(.7f, .77f, .84f); RenderSettings.fogDensity = .0035f;
        }
    }
}
