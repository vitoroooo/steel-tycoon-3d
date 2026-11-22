using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Builders
{
    /// <summary>The heavy-truck dealer: a glass showroom behind an open forecourt, one painted bay per truck.</summary>
    internal static class Dealers
    {
        /// <summary>Bays across the forecourt, and how far each truck stands in front of the lot's middle.</summary>
        internal const float BayWidth = 8, Hall = 7, Height = 7.5f;

        internal static void Trucks(TownKit kit, Lot lot)
        {
            float w = lot.Width, d = lot.Depth, back = -d / 2 + Hall / 2 + .5f, court = d - Hall - 1;
            Shapes.Flat(kit.Solid, lot, Tone.Concrete, 0, 0, w, d, Lot.Floor);   // solid: the trucks stand on it
            Wall[] walls = Shapes.Box(kit.Solid, lot, Tone.Cream, new Vector3(0, 0, back), new Vector3(w - 2, Height, Hall), Tone.Slate);
            walls[0].Panel(kit.Trim, Tone.Glass, 1, .5f, w - 3, Height - 2.8f);
            walls[0].Panel(kit.Trim, Tone.Yellow, 0, Height - 2.6f, w - 2, Height - 2.3f, .1f);
            walls[0].Panel(kit.Trim, Tone.Navy, 0, Height - 2.3f, w - 2, Height - .3f, .08f);
            walls[0].Awning(kit.Trim, Tone.Orange, 0, w - 2, Height - 2.8f, 1.6f, .3f);
            CityGeometry.Sign("DEALER TRUK & ALAT BERAT", lot.Point(0, Height - 1.3f, back + Hall / 2 + .12f), .8f,
                Tones.Colour(Tone.Yellow), lot.Yaw + 180);
            for (float line = -1.5f; line <= 1.5f; line++)
                Shapes.Flat(kit.Trim, lot, Tone.Yellow, line * BayWidth, d / 2 - court / 2 - .5f, .2f, court - 3, Lot.Floor + .02f);
            foreach (int side in new[] { -1, 1 })
            {
                Shapes.Box(kit.Solid, lot, Tone.Navy, new Vector3(side * (w / 2 - .1f), 0, d / 2 - court / 2), new Vector3(.2f, .9f, court), Tone.Yellow);
                Vector3 pole = lot.Point(side * (w / 2 - .8f), Lot.Foot, d / 2 - .8f);
                Shapes.Drum(kit.Solid, Tone.Slate, pole, .09f, .06f, 8, 5);
                Shapes.Face(kit.Trim, side < 0 ? Tone.Orange : Tone.Teal, pole + Vector3.up * 6.6f, pole + Vector3.up * 7.8f,
                    pole + Vector3.up * 7.8f + lot.Direction(-side * 1.9f, 0), pole + Vector3.up * 6.6f + lot.Direction(-side * 1.9f, 0),
                    lot.Direction(0, 1));
            }
        }
    }
}
