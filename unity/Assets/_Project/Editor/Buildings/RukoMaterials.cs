using System;
using UnityEngine;

namespace Steel.Editor.Buildings
{
    /// <summary>Maps the slots of Ruko.fbx to surfaces; each unit picks its own wall and shutter paint.</summary>
    internal static class RukoMaterials
    {
        private static readonly Vector2 Metres = Vector2.one;   // UVs are authored in metres

        /// <summary>Sun-faded exterior paints seen on Indonesian shophouses: wall, then shutter.</summary>
        private static readonly (string name, Color wall, Color shutter)[] Paints =
        {
            ("krem", new Color(.86f, .8f, .66f), new Color(.34f, .45f, .5f)),
            ("hijau", new Color(.62f, .76f, .66f), new Color(.55f, .56f, .55f)),
            ("biru", new Color(.6f, .74f, .84f), new Color(.5f, .27f, .2f)),
            ("kuning", new Color(.9f, .78f, .46f), new Color(.3f, .42f, .34f)),
            ("merah bata", new Color(.8f, .56f, .47f), new Color(.6f, .6f, .58f)),
            ("putih", new Color(.88f, .88f, .85f), new Color(.25f, .36f, .52f)),
        };

        internal static int PaintCount => Paints.Length;

        internal static Func<string, Material> Paint(int index)
        {
            var paint = Paints[index % Paints.Length];
            return slot =>
                slot == "wall" ? CityMaterials.Surface("beige_wall_001", Metres, "Ruko dinding " + paint.name, paint.wall) :
                slot == "shutter" ? CityMaterials.Surface("corrugated_iron", Metres, "Ruko rolling door " + paint.name,
                    paint.shutter) : Shared(slot);
        }

        private static Material Shared(string slot)
        {
            switch (slot)
            {
                case "backwall": return CityMaterials.Surface("beige_wall_001", Metres, "Ruko dinding belakang", new Color(.66f, .64f, .6f));
                case "trim": return CityMaterials.Surface("beige_wall_001", Metres, "Ruko lis", new Color(.93f, .92f, .88f));
                case "tile": return CityMaterials.Surface("worn_tile_floor", Metres, "Ruko teras");
                case "concrete": return CityMaterials.Surface("concrete_floor_02", Metres, "Ruko beton");
                case "awning": return CityMaterials.Surface("corrugated_iron", Metres, "Ruko kanopi seng");
                case "steel": return CityMaterials.Surface("rusty_painted_metal", Metres, "Ruko besi", new Color(.5f, .5f, .5f));
                case "steeldoor": return CityMaterials.Surface("green_metal_rust", Metres, "Ruko pintu besi");
                case "wood": return CityMaterials.Surface("weathered_planks", Metres, "Ruko bambu");
                case "frame": return CityMaterials.Plain("Ruko kusen", new Color(.2f, .16f, .13f), 0, .35f);
                case "door": return CityMaterials.Plain("Ruko pintu", new Color(.36f, .24f, .15f), 0, .4f);
                case "glass": return CityMaterials.Plain("Ruko kaca", new Color(.16f, .24f, .27f), .3f, .9f);
                case "sign": return CityMaterials.Plain("Ruko papan nama", new Color(.1f, .12f, .14f), .2f, .4f);
                case "appliance": return CityMaterials.Plain("Ruko AC", new Color(.82f, .82f, .78f), .1f, .45f);
                case "pipe": return CityMaterials.Plain("Ruko pipa PVC", new Color(.7f, .7f, .68f), 0, .4f);
                case "toren": return CityMaterials.Plain("Toren oranye", new Color(.9f, .42f, .08f), 0, .5f);
                case "lpg": return CityMaterials.Plain("Tabung LPG", new Color(.42f, .68f, .2f), .3f, .5f);
                case "cloth_red": return CityMaterials.Plain("Kain merah", new Color(.62f, .16f, .14f), 0, .1f);
                case "cloth_white": return CityMaterials.Plain("Kain putih", new Color(.85f, .84f, .8f), 0, .1f);
                case "cloth_blue": return CityMaterials.Plain("Kain biru", new Color(.18f, .32f, .55f), 0, .1f);
                case "cloth_yellow": return CityMaterials.Plain("Kain kuning", new Color(.85f, .68f, .2f), 0, .1f);
                default: throw new ArgumentException("Unknown ruko material slot: " + slot);
            }
        }
    }
}
