using UnityEngine;
using Steel.Editor.Plan.Places.Builders;
using Steel.Editor.Plan.Town;
using Steel.Presentation.World.Yard;
using Steel.Services.Map;

namespace Steel.Editor.Plan.Places.Haul
{
    /// <summary>What appears on the player's yard lot once it opens as a Kasta 2 yard: a heap per grade, the loader and the loading post.</summary>
    internal static class YardWorksBuilder
    {
        private static readonly Color[] HeapColours =
            { new Color(.42f, .47f, .52f), new Color(.62f, .33f, .2f), new Color(.5f, .42f, .34f), new Color(.2f, .55f, .52f) };

        internal static void Build(TownKit kit, Lot lot)
        {
            Shapes.Flat(kit.Trim, lot, Tone.Yellow, MapSite.PostRight, MapSite.PostForward, 1.6f, 1.6f, Lot.Floor + .02f);
            Props.Board(kit, lot.Part(MapSite.PostRight - 3.4f, MapSite.PostForward + 2, 4, 1), "POS MUAT");
            var works = new GameObject("Lapak pemain").AddComponent<YardWorks>();
            works.transform.SetParent(CityGeometry.Root, false);
            works.transform.SetPositionAndRotation(lot.Point(0, Lot.Floor, 0), Quaternion.Euler(0, lot.Yaw, 0));
            works.Heaps = new Transform[HeapColours.Length];
            float span = lot.Width - 12;
            for (int grade = 0; grade < works.Heaps.Length; grade++)
            {
                GameObject heap = CityGeometry.Shape("Stok grade " + grade, PrimitiveType.Sphere, Vector3.zero, Vector3.one,
                    CityMaterials.Plain("Stok lapak " + grade, HeapColours[grade]), false);
                heap.isStatic = false; heap.transform.SetParent(works.transform, false);
                heap.transform.localPosition = new Vector3(-span / 2 + span * grade / 3f - 2, 0, -lot.Depth / 2 + 5);
                works.Heaps[grade] = heap.transform;
            }
            works.Loader = LoaderModel.Create(works.transform);
            works.Rest = new Vector3(3, 0, -2); works.Dock = new Vector3(3, 0, lot.Depth / 2 - 3);
        }
    }
}
