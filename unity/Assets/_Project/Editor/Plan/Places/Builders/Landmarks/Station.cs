using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Builders.Landmarks
{
    /// <summary>The railway station: a long hall with a clock tower over the door and a roofed platform behind.</summary>
    internal static class Station
    {
        internal const string Board = "STASIUN SUKABAJA";
        private const float Height = 5, Tower = 10, Platform = 5;

        internal static void Build(TownKit kit, Lot lot)
        {
            float w = lot.Width - 3, d = Mathf.Max(5, lot.Depth - Platform - 7), middle = lot.Depth / 2 - 5 - d / 2;
            Shapes.Flat(kit.Trim, lot, Tone.Paving, 0, 0, lot.Width, lot.Depth, Lot.Floor);
            Wall[] hall = Shapes.Box(kit.Solid, lot, Tone.Cream, new Vector3(0, 0, middle), new Vector3(w, Height, d));
            Shapes.Roof(kit.Trim, lot, Tone.Terracotta, Tone.Cream, new Vector3(0, Height, middle), w + 1.4f, d + 1.4f, 2.4f, d * .5f);
            foreach (Wall face in hall) face.Panel(kit.Trim, Tone.Teal, 0, 0, face.Width, .8f, .04f);
            hall[0].Rows(kit.Trim, Tone.Glass, Mathf.Max(2, (int)(w / 3.2f)), 1, 1.4f, 0, 2.2f, .55f);
            hall[0].Awning(kit.Trim, Tone.Teal, 0, w, 3.9f, 1.8f, .3f);
            // The tower stands proud of the hall, with a clock on the street side.
            float front = middle + d / 2;
            Wall[] tower = Shapes.Box(kit.Solid, lot, Tone.White, new Vector3(0, 0, front), new Vector3(5, Tower, 3));
            Shapes.Roof(kit.Trim, lot, Tone.Terracotta, Tone.White, new Vector3(0, Tower, front), 6, 4, 2.6f, 3);
            tower[0].Panel(kit.Trim, Tone.Door, 1.2f, 0, 3.8f, 3.2f);
            tower[0].Panel(kit.Trim, Tone.Navy, 1.4f, Tower - 3.4f, 3.6f, Tower - 1.2f, .06f);
            tower[0].Panel(kit.Trim, Tone.Butter, 1.65f, Tower - 3.15f, 3.35f, Tower - 1.45f, .1f);
            CityGeometry.Sign(Board, lot.Point(0, Tower - 4.6f, front + 1.62f), .5f, Tones.Colour(Tone.Navy), lot.Yaw + 180);
            // The platform faces the tracks behind the hall: a raised slab under a seng roof on posts.
            float rear = middle - d / 2 - Platform / 2 - .2f;
            Shapes.Box(kit.Solid, lot, Tone.Concrete, new Vector3(0, 0, rear), new Vector3(w, .5f, Platform), Tone.Paving);
            Shapes.Flat(kit.Trim, lot, Tone.Yellow, 0, rear - Platform / 2 + .4f, w, .25f, .52f);
            Shapes.Box(kit.Trim, lot, Tone.Seng, new Vector3(0, 4, rear), new Vector3(w + 1, .25f, Platform + 1), Tone.Slate);
            for (float post = -w / 2 + 1; post <= w / 2 - .9f; post += 4)
                Shapes.Drum(kit.Solid, Tone.Teal, lot.Point(post, .5f, rear), .14f, .14f, 3.5f, 6);
        }
    }
}
