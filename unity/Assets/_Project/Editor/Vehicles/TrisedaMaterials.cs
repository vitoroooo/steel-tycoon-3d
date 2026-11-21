using UnityEngine;

namespace Steel.Editor.Vehicles
{
    /// <summary>Maps the material slots of Triseda.fbx: worn blue bodywork over a rust-streaked bed.</summary>
    internal static class TrisedaMaterials
    {
        private static readonly Vector2 Metres = Vector2.one;   // UVs are authored in metres
        private static readonly Color Body = new Color(1.25f, 1.25f, 1.3f);   // lifts the dark blue plate
        private static readonly Color Black = new Color(.3f, .3f, .32f);

        internal static Material For(string slot)
        {
            switch (slot)
            {
                case "paint": return CityMaterials.Surface("blue_metal_plate", Metres, "Triseda cat biru", Body);
                case "bed": return CityMaterials.Surface("rusty_painted_metal", Metres, "Triseda bak");
                case "bedfloor": return CityMaterials.Surface("metal_plate", Metres * 2, "Triseda lantai bak");
                case "frame": return CityMaterials.Surface("rusty_painted_metal", Metres, "Triseda rangka", Black);
                case "steel": return CityMaterials.Surface("rusty_metal_04", Metres, "Triseda besi");
                case "sack": return CityMaterials.Surface("hessian_230", Metres, "Triseda karung");
                case "jerrycan": return CityMaterials.Surface("green_metal_rust", Metres, "Triseda jeriken");
                case "engine": return CityMaterials.Plain("Triseda mesin", new Color(.5f, .5f, .5f), .85f, .42f);
                case "rim": return CityMaterials.Plain("Triseda pelek", new Color(.13f, .13f, .14f), .6f, .35f);
                case "chrome": return CityMaterials.Plain("Triseda krom", new Color(.78f, .78f, .8f), 1, .82f);
                case "rubber": return CityMaterials.Plain("Triseda karet", new Color(.035f, .035f, .04f), 0, .28f);
                case "seat": return CityMaterials.Plain("Triseda jok", new Color(.05f, .045f, .045f), 0, .45f);
                case "rope": return CityMaterials.Plain("Triseda tali", new Color(.55f, .44f, .27f), 0, .15f);
                case "glass": return CityMaterials.Plain("Triseda lensa", new Color(.86f, .88f, .84f), .2f, .92f);
                case "lamp_red": return CityMaterials.Plain("Triseda lampu rem", new Color(.55f, .04f, .03f), 0, .85f);
                case "lamp_amber": return CityMaterials.Plain("Triseda sein", new Color(.85f, .42f, .04f), 0, .85f);
                case "plate": return CityMaterials.Plain("Triseda pelat", new Color(.05f, .05f, .05f), 0, .4f);
                case "plate_text":
                case "stencil": return CityMaterials.Plain("Triseda cat putih", new Color(.88f, .85f, .76f), 0, .3f);
                default: throw new System.ArgumentException("Unknown triseda material slot: " + slot);
            }
        }
    }
}
