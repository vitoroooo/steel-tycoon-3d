using System;
using UnityEngine;

namespace Steel.Editor.Buildings
{
    /// <summary>Maps the slots of Rumah.fbx to surfaces; each house picks its own wall, dado and shutter paint.</summary>
    internal static class RumahMaterials
    {
        private static readonly Vector2 Metres = Vector2.one;   // UVs are authored in metres

        /// <summary>Kampung colour schemes: wall, the darker dado band below it, then shutters.</summary>
        private static readonly (string name, Color wall, Color dado, Color shutter)[] Paints =
        {
            ("putih hijau", new Color(.88f, .88f, .82f), new Color(.32f, .5f, .38f), new Color(.24f, .42f, .3f)),
            ("kuning coklat", new Color(.92f, .82f, .52f), new Color(.5f, .32f, .2f), new Color(.42f, .26f, .16f)),
            ("biru muda", new Color(.66f, .8f, .86f), new Color(.24f, .38f, .52f), new Color(.2f, .32f, .46f)),
            ("merah muda", new Color(.9f, .72f, .7f), new Color(.52f, .25f, .25f), new Color(.86f, .84f, .78f)),
            ("hijau pupus", new Color(.74f, .84f, .66f), new Color(.86f, .84f, .76f), new Color(.5f, .3f, .18f)),
        };

        internal static Func<string, Material> Paint(int index)
        {
            var paint = Paints[index % Paints.Length];
            return slot =>
                slot == "wall" ? CityMaterials.Surface("beige_wall_001", Metres, "Rumah dinding " + paint.name, paint.wall) :
                slot == "dado" ? CityMaterials.Surface("beige_wall_001", Metres, "Rumah dado " + paint.name, paint.dado) :
                slot == "shutter" ? CityMaterials.Plain("Rumah daun jendela " + paint.name, paint.shutter, 0, .35f) :
                Shared(slot);
        }

        private static Material Shared(string slot)
        {
            switch (slot)
            {
                case "roof": return CityMaterials.Surface("clay_roof_tiles_02", Metres, "Rumah genteng", new Color(.78f, .7f, .66f));
                case "seng": return CityMaterials.Surface("corrugated_iron", Metres, "Rumah seng");
                case "tile": return CityMaterials.Surface("worn_tile_floor", Metres, "Rumah teras");
                case "concrete": return CityMaterials.Surface("concrete_floor_02", Metres, "Rumah beton");
                case "wood": return CityMaterials.Surface("weathered_planks", Metres, "Rumah kayu", new Color(.6f, .45f, .32f));
                case "planks": return CityMaterials.Surface("weathered_planks", Metres, "Rumah papan");
                case "steel": return CityMaterials.Surface("rusty_painted_metal", Metres, "Rumah besi", new Color(.5f, .5f, .5f));
                case "bamboo": return CityMaterials.Plain("Bambu", new Color(.6f, .5f, .32f), 0, .4f);
                case "trim": return CityMaterials.Plain("Rumah lis", new Color(.9f, .89f, .84f), 0, .3f);
                case "frame": return CityMaterials.Plain("Rumah kusen", new Color(.3f, .2f, .13f), 0, .35f);
                case "glass": return CityMaterials.Plain("Rumah kaca", new Color(.14f, .2f, .22f), .3f, .9f);
                case "appliance": return CityMaterials.Plain("Parabola", new Color(.8f, .8f, .78f), .4f, .5f);
                case "drum": return CityMaterials.Plain("Drum plastik biru", new Color(.12f, .3f, .62f), 0, .55f);
                case "cloth_red": return CityMaterials.Plain("Kain merah", new Color(.62f, .16f, .14f), 0, .1f);
                case "cloth_white": return CityMaterials.Plain("Kain putih", new Color(.85f, .84f, .8f), 0, .1f);
                case "cloth_blue": return CityMaterials.Plain("Kain biru", new Color(.18f, .32f, .55f), 0, .1f);
                case "cloth_yellow": return CityMaterials.Plain("Kain kuning", new Color(.85f, .68f, .2f), 0, .1f);
                default: throw new ArgumentException("Unknown rumah material slot: " + slot);
            }
        }
    }
}
