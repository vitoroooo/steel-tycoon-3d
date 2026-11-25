using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Builders
{
    /// <summary>Industrial sheds: warehouses and workshops, and factories that add a striped chimney.</summary>
    internal static class Sheds
    {
        private static readonly Tone[] Skins = { Tone.Seng, Tone.Cream, Tone.White, Tone.Sky }, Tops = { Tone.Slate, Tone.Teal, Tone.Rust, Tone.Seng };

        internal static void Warehouse(TownKit kit, Lot lot) => Hall(kit, lot);

        internal static void Factory(TownKit kit, Lot lot)
        {
            float height = Hall(kit, lot);
            if (lot.Width < 18) return;
            Vector3 foot = lot.Point(lot.Width / 2 - 2.6f, Lot.Foot, -lot.Depth / 2 + 2.6f);
            // Red and white bands, as on every works chimney.
            for (int band = 0; band < 6; band++)
                Shapes.Drum(band == 0 ? kit.Solid : kit.Trim, band % 2 == 0 ? Tone.White : Tone.Brick,
                    foot + Vector3.up * band * height * .3f, 1.1f - band * .06f, 1.04f - band * .06f, height * .3f);
        }

        private static float Hall(TownKit kit, Lot lot)
        {
            float w = lot.Width - 3, d = lot.Depth - 5, back = -1.5f, height = 5.5f + lot.Roll(1) * 3;
            Tone skin = lot.Pick(2, Skins), top = lot.Pick(3, Tops), accent = lot.Pick(4, Tones.Accents);
            Shapes.Flat(kit.Trim, lot, Tone.Concrete, 0, 0, lot.Width, lot.Depth, Lot.Floor);
            Wall[] walls = Shapes.Box(kit.Solid, lot, skin, new Vector3(0, 0, back), new Vector3(w, height, d));
            Shapes.Roof(kit.Trim, lot, top, skin, new Vector3(0, height, back), w + .7f, d + .7f, d * .14f, 0);
            foreach (Wall face in walls)
            {
                face.Panel(kit.Trim, accent, 0, 0, face.Width, 1.3f, .03f);
                face.Rows(kit.Trim, Tone.Glass, Mathf.Max(1, (int)(face.Width / 4)), 1, height - 1.7f, 0, .9f, .7f);
            }
            float door = Mathf.Min(5, w * .45f);
            walls[0].Panel(kit.Trim, Tone.GlassDark, w / 2 - door / 2, 0, w / 2 + door / 2, Mathf.Min(4.4f, height - 2));
            walls[0].Panel(kit.Trim, Tone.Navy, w * .1f, height - .9f, w * .5f, height - .1f, .06f);
            for (int vent = 0; vent < 3 && w > 12; vent++)
                Shapes.Box(kit.Trim, lot, Tone.Seng, new Vector3((vent - 1) * w * .28f, height + d * .14f - .3f, back), new Vector3(1.6f, .9f, 1.2f), top);
            return height;
        }
    }
}
