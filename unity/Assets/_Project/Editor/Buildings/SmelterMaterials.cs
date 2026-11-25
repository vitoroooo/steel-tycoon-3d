using System;
using UnityEngine;

namespace Steel.Editor.Buildings
{
    /// <summary>Surfaces for the smelter: the melt shop, furnace, stacks, gantry crane, weighbridge, billets and scrap.</summary>
    internal static class SmelterMaterials
    {
        private static readonly Vector2 Metres = Vector2.one;   // UVs are authored in metres
        private static readonly Color Molten = new Color(1, .42f, .08f);

        internal static Material Paint(string slot) => Structure(slot) ?? Plant(slot) ?? Yard(slot)
            ?? throw new ArgumentException("Unknown smelter material slot: " + slot);

        /// <summary>Bars still glowing from the caster, for the bundles inside the hall.</summary>
        internal static Material HotBillet(string slot) =>
            slot == "billet" ? Glow("Billet panas", new Color(.95f, .3f, .06f), 2.2f) : Paint(slot);

        private static Material Structure(string slot)
        {
            switch (slot)
            {
                case "floor": return CityMaterials.Surface("concrete_floor_02", Metres, "Pabrik lantai", new Color(.55f, .55f, .54f));
                case "concrete": return CityMaterials.Surface("concrete_floor_02", Metres, "Pabrik beton", new Color(.8f, .8f, .78f));
                case "wall": return CityMaterials.Surface("cracked_concrete_wall", Metres, "Pabrik tembok");
                case "plaster": return CityMaterials.Surface("beige_wall_001", Metres, "Pabrik pos", new Color(.82f, .82f, .76f));
                case "frame": return CityMaterials.Surface("rusty_painted_metal", Metres, "Pabrik rangka", new Color(.5f, .62f, .68f));
                case "seng": return CityMaterials.Surface("corrugated_iron", Metres, "Pabrik seng", new Color(.74f, .78f, .8f));
                case "seng_rust": return CityMaterials.Surface("corrugated_iron", Metres, "Pabrik seng karat", new Color(.66f, .47f, .36f));
                case "roof": return CityMaterials.Surface("corrugated_iron", Metres, "Pabrik atap", new Color(.56f, .58f, .6f));
                case "door": return CityMaterials.Surface("green_metal_rust", Metres, "Pabrik pintu");
                case "deck": return CityMaterials.Surface("metal_plate", Metres, "Pabrik pelat lantai");
                case "duct": return CityMaterials.Surface("metal_plate", Metres, "Pabrik saluran asap", new Color(.7f, .7f, .68f));
                case "wood": return CityMaterials.Surface("weathered_planks", Metres, "Pabrik kayu");
                case "skylight": return CityMaterials.Plain("Pabrik fiber", new Color(.74f, .8f, .68f), 0, .55f);
                case "louvre": return CityMaterials.Plain("Pabrik kisi", new Color(.2f, .22f, .24f), .4f, .35f);
                case "sign": return CityMaterials.Plain("Pabrik papan", new Color(.08f, .1f, .12f), .2f, .4f);
                case "glass": return CityMaterials.Plain("Pabrik kaca", new Color(.16f, .24f, .27f), .3f, .9f);
                case "cabin": return CityMaterials.Plain("Pabrik kabin", new Color(.76f, .77f, .72f), .1f, .45f);
                case "steel_dark": return CityMaterials.Plain("Pabrik baja gelap", new Color(.1f, .11f, .12f), .6f, .4f);
                case "lamp": return Glow("Pabrik lampu", new Color(1, .93f, .78f), 2);
                default: return null;
            }
        }

        private static Material Plant(string slot)
        {
            switch (slot)
            {
                case "shell": return CityMaterials.Surface("rusty_metal_04", Metres, "Tungku cangkang", new Color(.56f, .5f, .47f));
                case "refractory": return CityMaterials.Surface("broken_brick_wall", Metres, "Tungku bata api", new Color(.5f, .4f, .35f));
                case "stack": return CityMaterials.Surface("rusty_painted_metal", Metres, "Cerobong", new Color(.62f, .6f, .58f));
                case "melt": return Glow("Baja cair", Molten, 5);
                case "beacon": return Glow("Lampu cerobong", new Color(1, .1f, .06f), 4);
                case "digits": return Glow("Angka timbangan", new Color(.95f, .2f, .05f), 3);
                case "graphite": return CityMaterials.Plain("Elektroda grafit", new Color(.11f, .11f, .12f), .1f, .55f);
                case "cable": return CityMaterials.Plain("Kabel tungku", new Color(.05f, .05f, .05f), 0, .3f);
                case "pipe": return CityMaterials.Plain("Pipa pendingin", new Color(.14f, .34f, .58f), .4f, .5f);
                case "crane": return CityMaterials.Plain("Derek kuning", new Color(.86f, .6f, .08f), .3f, .45f);
                case "rail": return CityMaterials.Plain("Pagar pengaman", new Color(.9f, .72f, .1f), .2f, .45f);
                case "magnet": return CityMaterials.Plain("Magnet angkat", new Color(.24f, .2f, .18f), .7f, .35f);
                case "band_red": return CityMaterials.Plain("Pita merah", new Color(.7f, .12f, .1f), .2f, .4f);
                case "band_white": return CityMaterials.Plain("Pita putih", new Color(.86f, .86f, .82f), .2f, .4f);
                default: return null;
            }
        }

        private static Material Yard(string slot)
        {
            switch (slot)
            {
                case "scrap": return CityMaterials.Surface("rusty_metal_04", Metres, "Skrap");
                case "paint_blue": return CityMaterials.Surface("blue_metal_plate", Metres, "Skrap cat biru");
                case "paint_red": return CityMaterials.Surface("rusty_painted_metal", Metres, "Skrap cat merah", new Color(.85f, .45f, .4f));
                case "rebar": return CityMaterials.Plain("Besi beton", new Color(.36f, .2f, .14f), .5f, .2f);
                case "billet": return CityMaterials.Plain("Billet", new Color(.2f, .22f, .26f), .8f, .5f);
                case "strap": return CityMaterials.Plain("Pita ikat", new Color(.08f, .08f, .09f), .6f, .5f);
                default: return null;
            }
        }

        private static Material Glow(string name, Color color, float strength)
        {
            var material = CityMaterials.Glow(name, color);
            material.SetColor("_EmissionColor", color * strength);   // bright enough to bloom
            return material;
        }
    }
}
