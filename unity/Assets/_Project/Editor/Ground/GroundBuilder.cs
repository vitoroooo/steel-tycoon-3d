using UnityEditor;
using UnityEngine;
using Steel.Services;

namespace Steel.Editor.Ground
{
    /// <summary>Builds the terrain under the whole city: shape and painted layers; GroundCover adds the grass once the city stands.</summary>
    internal static class GroundBuilder
    {
        private const int HeightSamples = 513, PaintSamples = 1024;
        private const string DataPath = CityMaterials.Generated + "Tanah.asset";
        private const string MaterialPath = CityMaterials.Generated + "Tanah.mat";

        internal static Terrain Create(CityLayout layout)
        {
            var zones = new GroundZones(layout);
            AssetDatabase.DeleteAsset(DataPath);   // rebuilt from scratch so stale detail layers never linger
            var data = new TerrainData { heightmapResolution = HeightSamples, alphamapResolution = PaintSamples };
            data.size = new Vector3(GroundZones.Size, GroundZones.Depth, GroundZones.Size);
            AssetDatabase.CreateAsset(data, DataPath);
            data.SetHeights(0, 0, GroundShape.Heights(zones, HeightSamples));
            data.terrainLayers = GroundLayers.Create();
            data.SetAlphamaps(0, 0, GroundPaint.Weights(zones, PaintSamples));

            var ground = Terrain.CreateTerrainGameObject(data);
            ground.name = "Tanah Sukabaja";
            ground.transform.SetParent(CityGeometry.Root);
            ground.transform.position = GroundZones.Centre
                + new Vector3(-GroundZones.Size / 2, GroundZones.CityLevel - GroundZones.Base, -GroundZones.Size / 2);
            ground.isStatic = true;
            var terrain = ground.GetComponent<Terrain>();
            terrain.materialTemplate = Material();
            terrain.heightmapPixelError = 6;
            terrain.basemapDistance = 140;
            terrain.detailObjectDistance = GroundCover.ViewDistance;
            terrain.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            EditorUtility.SetDirty(data);
            return terrain;
        }

        private static Material Material()
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material) return material;
            material = new Material(Shader.Find("Universal Render Pipeline/Terrain/Lit"));
            AssetDatabase.CreateAsset(material, MaterialPath);
            return material;
        }
    }
}
