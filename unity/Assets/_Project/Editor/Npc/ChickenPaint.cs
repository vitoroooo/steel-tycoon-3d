using System;
using UnityEngine;

namespace Steel.Editor.Npc
{
    internal static class ChickenPaint
    {
        internal static Material Paint(string slot, bool hen)
        {
            Color32 color;
            switch (slot)
            {
                case "feather_body": color = hen ? Rgb(137, 97, 69) : Rgb(78, 47, 36); break;
                case "feather_neck": color = hen ? Rgb(182, 134, 76) : Rgb(189, 107, 38); break;
                case "feather_tail": color = hen ? Rgb(80, 61, 50) : Rgb(41, 62, 54); break;
                case "feather_wing": color = hen ? Rgb(109, 82, 57) : Rgb(129, 66, 44); break;
                case "comb": color = Rgb(174, 43, 32); break;
                case "beak": color = Rgb(193, 159, 97); break;
                case "shank": color = Rgb(164, 143, 82); break;
                case "eye": color = Rgb(27, 21, 15); break;
                default: throw new ArgumentException("Unknown plumage slot: " + slot);
            }
            return CityMaterials.Plain((hen ? "Babon " : "Jago ") + slot, color, 0, slot == "eye" ? .75f : .25f);
        }

        private static Color32 Rgb(byte r, byte g, byte b) => new Color32(r, g, b, 255);
    }
}
