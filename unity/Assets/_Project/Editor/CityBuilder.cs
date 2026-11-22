using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Steel.Services;
using Steel.Presentation.Input;
using Steel.Presentation.Player;
using Steel.Presentation.UI;
using Steel.Presentation.World;

namespace Steel.Editor
{
    public static class CityBuilder
    {
        [MenuItem("Steel/Generate City")]
        public static void Generate()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var layout = new CityLayout(new ResourceConfigSource());
            CityGeometry.Root = new GameObject("Sukabaja").transform;
            CityLighting.Create();
            var ground = Ground.GroundBuilder.Create(layout);
            StreetBuilder.Create();
            Map.RukoStreets.Create(); Map.KampungLane.Create(); Map.Greenery.Create(); Map.Fences.Create();
            Map.BackAlley.Create(Map.RukoStreets.AlleyX, Map.RukoStreets.AlleySouth, Map.RukoStreets.AlleyNorth, 2.6f);
            FacadeDetails.Street();
            OverheadWires.Create();
            CityBoundsBuilder.Create(layout);
            YardBuilder.Create(layout); PileBuilder.Create(layout);
            Map.Smelter.Create(); Map.SmelterYard.Create(layout); Map.ContainerDepot.Create(); Map.DemolitionSite.Create(); Map.Townsfolk.Create();
            Ground.GroundDecals.Create(layout); Ground.GroundCover.Plant(ground, layout);
            var host = new GameObject("Game Services"); host.AddComponent<GameHost>();
            host.AddComponent<SliceInput>(); host.AddComponent<ScreenshotCapture>();
            Transform player = Characters.PlayerBuilder.Create(layout);
            Map.Livestock.Create(player);
            Vehicles.GerobakBuilder.Create(player);
            Vehicles.DealerBuilder.Create();   // the lot floor first: vehicles park on whatever is under them
            Vehicles.TrisedaBuilder.Create(player); Vehicles.PikapBuilder.Create(player);
            Vehicles.Trucks.TruckPool.Create(player);
            var camera = new GameObject("Main Camera", typeof(Camera)); camera.tag = "MainCamera";
            camera.AddComponent<AudioListener>(); camera.GetComponent<Camera>().fieldOfView = 62;
            camera.GetComponent<Camera>().nearClipPlane = .1f; camera.GetComponent<Camera>().farClipPlane = 200;
            var data = camera.AddComponent<UniversalAdditionalCameraData>(); data.renderPostProcessing = false;
            var orbit = camera.AddComponent<OrbitCamera>(); orbit.Target = player;
            new GameObject("HUD").AddComponent<CityHud>();
            var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            settings.FindProperty("activeInputHandler").intValue = 1; settings.ApplyModifiedPropertiesWithoutUndo();
            PlayerSettings.productName = "Steel Tycoon 3D";
            PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            EditorSceneManager.SaveScene(scene, Packaging.BuildScenes.City);
            Packaging.BuildScenes.Register();
            AssetDatabase.SaveAssets();
            Debug.Log("CITY_GENERATED: " + layout.Piles.Length + " piles, lapak, smelter, toon, flat sky, no post-processing");
        }
    }
}
