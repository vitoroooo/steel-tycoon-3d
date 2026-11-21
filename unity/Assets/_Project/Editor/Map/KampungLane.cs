using UnityEngine;
using Steel.Editor.Buildings;
using Steel.Editor.Models;

namespace Steel.Editor.Map
{
    /// <summary>Kampung houses along the west of the market lane, a metre apart so the gaps work as footpaths.</summary>
    internal static class KampungLane
    {
        private const float Pitch = 7, PorchReach = 4.1f, YardReach = 6.2f, Yaw = 90;
        private const int Houses = 9;
        private const float WestEdge = -54;
        private static readonly (string model, float height, float along)[] PorchPlants =
        {
            ("potted_plant_04", .85f, 2.2f), ("fern_02", .5f, -2.5f),
        };

        internal static void Create()
        {
            float east = RukoStreets.LaneWest, x = east - PorchReach;
            Quaternion facing = Quaternion.Euler(0, Yaw, 0);
            Ground(east);
            for (int i = 0; i < Houses; i++)
            {
                var house = AuthoredModel.Create("Buildings/Rumah", RumahMaterials.Paint(i * 2 + i / 3));
                house.name = "Rumah kampung " + (i + 1);
                house.transform.SetParent(CityGeometry.Root, false);
                house.transform.SetPositionAndRotation(new Vector3(x, 0, RukoStreets.LaneNorth - 4 - i * Pitch), facing);
                foreach (Transform part in house.GetComponentsInChildren<Transform>()) part.gameObject.isStatic = true;
                Solid(house);
                var plant = PorchPlants[i % PorchPlants.Length];
                ModelLibrary.Place(plant.model, house.transform.TransformPoint(plant.along, .15f, 3.4f), plant.height, i * 40);
            }
        }

        /// <summary>Packed earth from the lane to the city edge, level with the house floors' footing.</summary>
        private static void Ground(float east)
        {
            float south = RukoStreets.LaneSouth, north = RukoStreets.LaneNorth, width = east - WestEdge;
            var earth = CityMaterials.Surface("brown_mud_02", new Vector2(width / 4, (north - south) / 4),
                "Kampung tanah padat", new Color(.62f, .58f, .5f));
            CityGeometry.Box("Kampung / tanah", new Vector3(east - width / 2, -.1f, (south + north) / 2),
                new Vector3(width, .2f, north - south), earth);
        }

        /// <summary>Walls, the porch floor and the kitchen's half walls; the roof overhang stays walkable below.</summary>
        private static void Solid(GameObject house)
        {
            Box(house, new Vector3(0, 1.5f, -.6f), new Vector3(6, 3.05f, 5.6f));
            Box(house, new Vector3(0, .075f, 3), new Vector3(6, .15f, 1.6f));
            Box(house, new Vector3(-1.4f, .6f, -4.3f), new Vector3(3, 1.2f, 1.8f));
        }

        private static void Box(GameObject house, Vector3 centre, Vector3 size)
        {
            var box = house.AddComponent<BoxCollider>();
            box.center = centre; box.size = size;
        }
    }
}
