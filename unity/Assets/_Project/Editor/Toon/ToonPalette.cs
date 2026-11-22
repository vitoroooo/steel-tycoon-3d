using System.Collections.Generic;
using UnityEngine;

namespace Steel.Editor.Toon
{
    /// <summary>The one palette of Sukabaja. Every surface name used by the builders maps to a flat colour here.</summary>
    internal static class ToonPalette
    {
        internal static readonly Color Navy = Hex("1F2F4A"), Yellow = Hex("F2B632"), Orange = Hex("E8743B");
        internal static readonly Color Teal = Hex("2E8C86"), Cream = Hex("F4EBDD"), Ink = Hex("1A1C24");

        private static readonly Dictionary<string, Color> Surfaces = new Dictionary<string, Color>
        {
            { "asphalt_02", Hex("4A5060") }, { "beige_wall_001", Hex("F1E3C8") },
            { "blue_metal_plate", Hex("3F7FB5") }, { "blue_painted_planks", Hex("2E8C86") },
            { "broken_brick_wall", Hex("C4674A") }, { "brown_mud_02", Hex("B98F62") },
            { "clay_roof_tiles_02", Hex("D0603A") }, { "concrete_debris", Hex("A9A59C") },
            { "concrete_floor_02", Hex("CFC8BA") }, { "container_side", Hex("E8743B") },
            { "corrugated_iron", Hex("9DB0BA") }, { "cracked_concrete_wall", Hex("BDB6A8") },
            { "green_metal_rust", Hex("5E9C4B") }, { "hessian_230", Hex("D9C29A") },
            { "metal_plate", Hex("8D949B") }, { "rusty_metal_04", Hex("9A5230") },
            { "rusty_painted_metal", Hex("B8693C") }, { "weathered_planks", Hex("B07B4A") },
            { "worn_tile_floor", Hex("E5DCCB") },
        };

        /// <summary>The flat colour of a named surface; a tint repaints it but keeps a little of the base.</summary>
        internal static Color Surface(string id, Color? tint)
        {
            Color colour = Surfaces.TryGetValue(id, out Color known) ? known : Cream;
            return tint.HasValue ? Color.Lerp(colour, tint.Value, .7f) : colour;
        }

        internal static Color Hex(string code) =>
            ColorUtility.TryParseHtmlString("#" + code, out Color colour) ? colour : Color.magenta;
    }
}
