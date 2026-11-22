using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Builders.Landmarks
{
    /// <summary>The toll plaza: booths on kerbed islands under one long roof, with the toll office behind.</summary>
    internal static class TollGate
    {
        internal const string Board = "GERBANG TOL SUKABAJA";
        private const float Clear = 5.6f, LaneWidth = 5, Office = 5;

        internal static void Build(TownKit kit, Lot lot)
        {
            float w = lot.Width, d = lot.Depth, plaza = Mathf.Max(5, d - Office - 3), middle = d / 2 - 1 - plaza / 2;
            Shapes.Flat(kit.Solid, lot, Tone.Asphalt, 0, 0, w, d, Lot.Floor);   // solid: trucks queue on it
            int booths = Mathf.Max(2, (int)((w - 2) / LaneWidth));
            for (int booth = 0; booth < booths; booth++)
            {
                float x = (booth - (booths - 1) / 2f) * LaneWidth;
                Shapes.Box(kit.Solid, lot, Tone.Yellow, new Vector3(x, 0, middle), new Vector3(1.5f, .3f, plaza * .8f), Tone.Ink);
                Wall[] cabin = Shapes.Box(kit.Solid, lot, Tone.Cream, new Vector3(x, .3f, middle), new Vector3(1.3f, 2.5f, 2.4f), Tone.Teal);
                foreach (Wall face in cabin) face.Panel(kit.Trim, Tone.Glass, .12f, 1, face.Width - .12f, 2.1f, .03f);
                Shapes.Drum(kit.Solid, Tone.White, lot.Point(x, .3f, middle + 2), .12f, .12f, Clear - .3f, 6);
                Shapes.Drum(kit.Solid, Tone.White, lot.Point(x, .3f, middle - 2), .12f, .12f, Clear - .3f, 6);
                // A green or red lamp over each lane tells drivers which booth is open.
                Shapes.Box(kit.Trim, lot, booth % 3 == 2 ? Tone.Brick : Tone.Leaf, new Vector3(x + LaneWidth / 2, Clear - .7f, middle + plaza * .3f),
                    new Vector3(.7f, .7f, .2f), Tone.Ink);
            }
            Wall[] roof = Shapes.Box(kit.Trim, lot, Tone.Teal, new Vector3(0, Clear, middle), new Vector3(w - 1, 1.1f, plaza * .6f), Tone.White);
            roof[0].Panel(kit.Trim, Tone.Navy, 0, .15f, w - 1, .95f, .03f);
            CityGeometry.Sign(Board, lot.Point(0, Clear + .55f, middle + plaza * .3f + .1f), .55f, Tones.Colour(Tone.Yellow), lot.Yaw + 180);
            float back = -d / 2 + Office / 2 + .5f;
            Wall[] office = Shapes.Box(kit.Solid, lot, Tone.White, new Vector3(0, 0, back), new Vector3(w * .6f, 3.6f, Office));
            Shapes.Roof(kit.Trim, lot, Tone.Teal, Tone.White, new Vector3(0, 3.6f, back), w * .6f + 1, Office + 1, 1.6f, Office * .4f);
            office[0].Rows(kit.Trim, Tone.Glass, Mathf.Max(2, (int)(w * .6f / 3)), 1, 1.1f, 0, 1.5f, .55f);
            office[0].Panel(kit.Trim, Tone.Navy, 0, 0, w * .6f, .7f, .04f);
        }
    }
}
