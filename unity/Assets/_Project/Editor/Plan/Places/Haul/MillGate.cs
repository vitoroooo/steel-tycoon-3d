using UnityEngine;
using Steel.Editor.Plan.Places.Builders;
using Steel.Editor.Plan.Town;
using Steel.Presentation.World.Gate;
using Steel.Services.Map;

namespace Steel.Editor.Plan.Places.Haul
{
    /// <summary>The gate of PT Baja Nusantara: a weighbridge trucks drive onto, a weighing post and a magnet crane over the bay.</summary>
    internal static class MillGate
    {
        private const float DeckWidth = 4.2f, CraneZ = -5, CraneHeight = 7.5f, LaneX = 9;

        internal static void Build(TownKit kit, Lot lot)
        {
            float w = lot.Width, d = lot.Depth;
            Shapes.Flat(kit.Solid, lot, Tone.Concrete, 0, 0, w, d, Lot.Floor);   // solid: trucks stand on it
            Shapes.Flat(kit.Trim, lot, Tone.Slate, 0, 0, DeckWidth, d - 2, Lot.Floor + .02f);
            foreach (int side in new[] { -1, 1 })
            {
                Shapes.Flat(kit.Trim, lot, Tone.Yellow, side * (DeckWidth / 2 + .15f), 0, .3f, d - 2, Lot.Floor + .03f);
                Shapes.Drum(kit.Solid, Tone.Navy, lot.Point(side * 4.4f, Lot.Foot, CraneZ), .28f, .22f, CraneHeight + .2f, 6);
            }
            Shapes.Box(kit.Trim, lot, Tone.Yellow, new Vector3(0, CraneHeight, CraneZ), new Vector3(9.6f, .6f, .7f), Tone.Navy);
            Shapes.Drum(kit.Trim, Tone.Ink, lot.Point(0, CraneHeight - 2.4f, CraneZ), .05f, .05f, 2.4f, 4);
            Shapes.Drum(kit.Trim, Tone.Orange, lot.Point(0, CraneHeight - 2.9f, CraneZ), .9f, .7f, .5f, 10);
            Wall[] booth = Shapes.Box(kit.Solid, lot, Tone.Cream, new Vector3(MapSite.PostRight - 3.2f, 0, MapSite.PostForward),
                new Vector3(3.4f, 2.9f, 3.4f), Tone.Navy);
            booth[1].Panel(kit.Trim, Tone.Glass, .5f, 1, 2.9f, 2.2f);
            booth[1].Awning(kit.Trim, Tone.Orange, 0, 3.4f, 2.5f, 1, .25f);
            Props.Board(kit, lot.Part(-w / 2 + 4.5f, d / 2 - .8f, 8, 1), "PT BAJA NUSANTARA\nJEMBATAN TIMBANG");
            Shapes.Flat(kit.Trim, lot, Tone.Yellow, MapSite.PostRight, MapSite.PostForward, 1.6f, 1.6f, Lot.Floor + .02f);
            Queue(lot);
        }

        /// <summary>The waiting lane beside the bridge, with the trucks that fill it when the gate is busy.</summary>
        private static void Queue(Lot lot)
        {
            var line = new GameObject("Antrean gerbang").AddComponent<GateLine>();
            line.transform.SetParent(CityGeometry.Root, false);
            line.transform.SetPositionAndRotation(lot.Point(LaneX, Lot.Floor, 0), Quaternion.Euler(0, lot.Yaw, 0));
            line.Trucks = new GameObject[3];
            for (int i = 0; i < line.Trucks.Length; i++)
            {
                line.Trucks[i] = QueueTruck.Create(line.transform, i);
                line.Trucks[i].transform.localPosition = new Vector3(0, 0, -lot.Depth / 2 + 4.5f + i * 6.8f);
            }
        }
    }
}
