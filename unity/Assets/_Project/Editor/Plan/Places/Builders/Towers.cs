using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Builders
{
    /// <summary>Pusat Kota's tall blocks: a glass lobby, a window on every floor, a coloured fin and plant on the roof.</summary>
    internal static class Towers
    {
        private const float Storey = 3.1f, Lobby = 4.2f;

        internal static void Build(TownKit kit, Lot lot)
        {
            float w = lot.Width - 2, d = lot.Depth - 2;
            int floors = 3 + lot.Pick(1, 6) + (lot.Width > 18 ? 4 : 0);
            float height = Lobby + floors * Storey;
            Tone wall = lot.Pick(2, Tones.Walls), accent = lot.Pick(3, Tones.Accents);
            Shapes.Flat(kit.Trim, lot, Tone.Paving, 0, 0, lot.Width, lot.Depth, Lot.Floor);
            Wall[] walls = Shapes.Box(kit.Solid, lot, wall, Vector3.zero, new Vector3(w, height, d), Tone.Concrete);
            foreach (Wall face in walls)
                face.Rows(kit.Trim, Tone.Glass, Mathf.Max(1, (int)(face.Width / 2.6f)), floors, Lobby + .9f, Storey, 1.6f);
            walls[0].Panel(kit.Trim, Tone.GlassDark, 1, 0, w - 1, 3.2f);
            walls[0].Awning(kit.Trim, accent, .5f, w - .5f, 3.7f, 1.8f, .15f);
            walls[0].Panel(kit.Trim, accent, w * .06f, Lobby, w * .06f + 1.1f, height, .12f);
            Shapes.Box(kit.Trim, lot, accent, new Vector3(0, height, 0), new Vector3(w + .5f, .7f, d + .5f));
            Shapes.Box(kit.Trim, lot, Tone.Slate, new Vector3(w * .2f, height, -d * .15f), new Vector3(3, 2.6f, 3), Tone.Concrete);
            Shapes.Drum(kit.Trim, Tone.Orange, lot.Point(-w * .25f, height, d * .1f), .9f, .9f, 1.5f);
        }
    }
}
