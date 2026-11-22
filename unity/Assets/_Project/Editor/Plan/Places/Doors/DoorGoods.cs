using UnityEngine;
using Steel.Core.Trade.Street;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Doors
{
    /// <summary>The scrap waiting at a door, as a few flat-coloured pieces that vanish together once it is sold.</summary>
    internal static class DoorGoods
    {
        private static readonly int[] Pieces = { 3, 5, 8, 5 };          // by SellerKind: a house has little, a project a lot
        private static readonly float[] Reach = { .7f, 1, 1.5f, 1 };

        internal static void Fill(Transform heap, SellerKind kind, Lot lot)
        {
            Material rust = CityMaterials.Plain("Rongsok karat", new Color(.62f, .33f, .2f)),
                steel = CityMaterials.Plain("Rongsok baja", new Color(.42f, .47f, .52f));
            float reach = Reach[(int)kind];
            for (int i = 0; i < Pieces[(int)kind]; i++)
            {
                float x = (lot.Roll(20 + i) - .5f) * reach * 1.6f, z = (lot.Roll(40 + i) - .5f) * reach * 1.2f;
                bool bar = i % 3 == 1, drum = i % 3 == 2;
                Vector3 size = bar ? new Vector3(.12f, .12f, reach * 1.3f) : drum ? new Vector3(.5f, .4f, .5f) :
                    new Vector3(.8f, .06f, .55f);
                GameObject piece = CityGeometry.Shape("Barang", drum ? PrimitiveType.Cylinder : PrimitiveType.Cube,
                    Vector3.zero, size, i % 2 == 0 ? rust : steel, false);
                piece.transform.SetParent(heap, false);
                piece.transform.localPosition = new Vector3(x, (drum ? .4f : .08f) + i * .07f, z);
                piece.transform.localRotation = Quaternion.Euler(bar ? 6 : 0, lot.Roll(60 + i) * 180, drum ? 0 : 5);
            }
            var blocker = heap.gameObject.AddComponent<BoxCollider>();   // one simple shape stops feet and wheels
            blocker.center = new Vector3(0, .4f, 0); blocker.size = new Vector3(reach * 1.8f, .8f, reach * 1.4f);
        }
    }
}
