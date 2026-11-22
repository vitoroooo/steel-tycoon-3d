using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Steel.Editor.Toon;
using Steel.Presentation.World;

namespace Steel.Editor.Atmosphere
{
    /// <summary>The painted sky, the sun that casts the city's one shadow tone, ambient light and haze.</summary>
    internal static class SkyDome
    {
        private const float SunPitch = 47.8f;
        /// <summary>Compass bearing of the sun in the world; shadows fall away from it.</summary>
        private const float SunBearing = 145;
        private static readonly Color Zenith = ToonPalette.Hex("3F8FD8"), Horizon = ToonPalette.Hex("C9E8F4");
        private static readonly Color Haze = ToonPalette.Hex("BFDDEA");

        internal static Light Create()
        {
            var sun = new GameObject("Langit / Matahari").AddComponent<Light>();
            sun.type = LightType.Directional; sun.color = new Color(1, .96f, .88f);
            sun.intensity = 1; sun.shadows = LightShadows.Hard; sun.shadowStrength = 1;
            sun.transform.rotation = Quaternion.Euler(SunPitch, SunBearing + 180, 0);
            RenderSettings.sun = sun;
            RenderSettings.skybox = Sky();
            Ambient();
            sun.gameObject.AddComponent<SkyDrift>();
            return sun;
        }

        private static Material Sky()
        {
            string path = CityMaterials.Generated + "Langit.mat";
            AssetDatabase.DeleteAsset(path);   // the old panoramic material keeps its HDR picture otherwise
            var sky = new Material(Shader.Find("Steel/ToonSky"));
            AssetDatabase.CreateAsset(sky, path);
            sky.SetColor("_Zenith", Zenith); sky.SetColor("_Horizon", Horizon); sky.SetColor("_Ground", Haze);
            sky.SetTexture("_CloudMap",
                AssetDatabase.LoadAssetAtPath<Texture>("Assets/_Project/Textures/Sky/cloud_shadow.png"));
            return sky;
        }

        private static void Ambient()
        {
            // Only the terrain and a few textured props still read ambient light; toon surfaces ignore it.
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.3f, .32f, .36f);
            RenderSettings.fog = true; RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = Haze; RenderSettings.fogStartDistance = 55; RenderSettings.fogEndDistance = 190;
        }
    }
}
