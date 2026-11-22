using UnityEngine;

namespace Steel.Editor.Plan.Town
{
    /// <summary>Every colour the generated town is painted in. The order is the order of swatches in the palette map.</summary>
    internal enum Tone
    {
        Cream, Peach, Mint, Sky, Butter, Rose, White, Concrete,
        Terracotta, Brick, Slate, Seng, Rust, Navy, Yellow, Orange,
        Teal, Ink, Glass, GlassDark, Door, Wood, Leaf, LeafDark,
        Grass, Field, Trunk, Dirt, Paving, Asphalt, Water, Rock
    }

    internal static class Tones
    {
        internal const int Columns = 8, Rows = 4;
        private static readonly string[] Codes =
        {
            "F4EBDD", "F6C9A8", "BFE3D3", "BFD9EE", "F7E3A1", "F2B8B0", "FBF7EE", "CFC9BC",
            "C8553D", "A8452F", "5B6478", "A9B4BD", "A8553A", "1F2F4A", "F2B632", "E8743B",
            "2E8C86", "1A1C24", "8FD0E8", "3E6C9A", "7A4B2E", "B98A5A", "6FB04A", "4E9A45",
            "9CCB5C", "C9DC6A", "7A5536", "D9C39A", "E2D6BF", "4A4F5E", "5FB8D6", "B8AE9C"
        };

        internal static readonly Tone[] Walls = { Tone.Cream, Tone.Peach, Tone.Mint, Tone.Sky, Tone.Butter, Tone.Rose, Tone.White };
        internal static readonly Tone[] Roofs = { Tone.Terracotta, Tone.Brick, Tone.Slate, Tone.Terracotta, Tone.Teal };
        internal static readonly Tone[] Accents = { Tone.Yellow, Tone.Orange, Tone.Teal, Tone.Navy };

        internal static Color Colour(Tone tone) => Toon.ToonPalette.Hex(Codes[(int)tone]);

        /// <summary>Where this tone sits on the palette map.</summary>
        internal static Vector2 Uv(Tone tone) =>
            new Vector2(((int)tone % Columns + .5f) / Columns, ((int)tone / Columns + .5f) / Rows);
    }
}
