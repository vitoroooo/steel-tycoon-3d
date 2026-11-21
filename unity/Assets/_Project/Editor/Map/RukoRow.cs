using UnityEngine;
using Steel.Editor.Buildings;
using Steel.Editor.Models;

namespace Steel.Editor.Map
{
    /// <summary>A terrace of ruko units sharing side walls, each with its own paint and shop sign.</summary>
    internal static class RukoRow
    {
        internal const float UnitWidth = 5, Depth = 9;
        /// <summary>How far the step in front and the apron behind reach past the footprint centre.</summary>
        internal const float FrontReach = 5.3f, BackReach = 5.7f;
        private const float ShopHeight = 3.5f, StoreyHeight = 3.2f, Parapet = .84f, Terrace = 1.1f;
        private const float SignHeight = 4.05f, SignSize = .15f;
        private static readonly Color SignText = new Color(1, .86f, .55f);

        /// <summary>Builds one unit per shop name, from <paramref name="first"/> towards the row's right.</summary>
        internal static void Create(Vector3 first, float yaw, string[] shops, int paint = 0)
        {
            Quaternion facing = Quaternion.Euler(0, yaw, 0);
            Vector3 step = facing * Vector3.right * UnitWidth;
            Pad(first + step * (shops.Length - 1) / 2f, facing, shops.Length * UnitWidth);
            for (int i = 0; i < shops.Length; i++)
            {
                int floors = i % 3 == 1 ? 3 : 2;   // every third unit is taller, so the roofline steps
                var unit = AuthoredModel.Create(floors == 3 ? "Buildings/Ruko3" : "Buildings/Ruko",
                    RukoMaterials.Paint(paint + i));
                unit.name = "Ruko / " + shops[i];
                unit.transform.SetParent(CityGeometry.Root, false);
                unit.transform.SetPositionAndRotation(first + step * i, facing);
                foreach (Transform part in unit.GetComponentsInChildren<Transform>()) part.gameObject.isStatic = true;
                Solid(unit, floors);
                CityGeometry.Sign(shops[i], unit.transform.TransformPoint(0, SignHeight, Depth / 2 + .12f),
                    SignSize, SignText, yaw + 180);
            }
        }

        /// <summary>The slab the terrace stands on, level with the unit floors' street edge.</summary>
        private static void Pad(Vector3 middle, Quaternion facing, float length)
        {
            var concrete = CityMaterials.Surface("concrete_floor_02", new Vector2(length / 3, 4),
                "Ruko pelat " + Mathf.RoundToInt(length) + " m", new Color(.75f, .78f, .8f));
            var pad = CityGeometry.Box("Ruko / pelat", middle + facing * Vector3.forward * (FrontReach - BackReach) / 2
                + Vector3.down * .15f, new Vector3(length, .3f, FrontReach + BackReach), concrete);
            pad.transform.rotation = facing;
        }

        /// <summary>The walls block movement; the raised terrace under the upper floors stays walkable.</summary>
        private static void Solid(GameObject unit, int floors)
        {
            float height = ShopHeight + (floors - 1) * StoreyHeight + Parapet;
            var body = unit.AddComponent<BoxCollider>();
            body.center = new Vector3(0, height / 2, -Terrace / 2);
            body.size = new Vector3(UnitWidth, height, Depth - Terrace);
            var terrace = unit.AddComponent<BoxCollider>();
            terrace.center = new Vector3(0, .09f, Depth / 2 - Terrace / 2 + .25f);
            terrace.size = new Vector3(UnitWidth, .18f, Terrace + .5f);
        }
    }
}
