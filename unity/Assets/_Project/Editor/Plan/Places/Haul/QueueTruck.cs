using UnityEngine;

namespace Steel.Editor.Plan.Places.Haul
{
    /// <summary>A parked supplier's truck from a few flat-coloured shapes, loaded with scrap; forward is +Z.</summary>
    internal static class QueueTruck
    {
        private static readonly Color[] Cabs = { new Color(.18f, .55f, .52f), new Color(.91f, .45f, .23f), new Color(.95f, .71f, .2f) };

        internal static GameObject Create(Transform parent, int index)
        {
            var truck = new GameObject("Truk antre " + (index + 1)).transform;
            truck.SetParent(parent, false);
            Material cab = CityMaterials.Plain("Truk antre kabin " + index % Cabs.Length, Cabs[index % Cabs.Length]),
                bed = CityMaterials.Plain("Loader navy", new Color(.12f, .18f, .29f)),
                load = CityMaterials.Plain("Rongsok karat", new Color(.62f, .33f, .2f)),
                tyre = CityMaterials.Plain("Loader ban", new Color(.1f, .11f, .14f));
            Part(truck, PrimitiveType.Cube, new Vector3(0, 1.45f, 2.2f), new Vector3(2.2f, 1.9f, 1.7f), cab);
            Part(truck, PrimitiveType.Cube, new Vector3(0, 1.15f, -.9f), new Vector3(2.3f, 1.1f, 4.4f), bed);
            Part(truck, PrimitiveType.Cube, new Vector3(0, 1.95f, -.9f), new Vector3(2, .7f, 4), load);
            foreach (int side in new[] { -1, 1 })
                foreach (float z in new[] { 2.1f, -2 })
                    Part(truck, PrimitiveType.Cylinder, new Vector3(side * 1.05f, .5f, z), new Vector3(1, .18f, 1), tyre)
                        .localRotation = Quaternion.Euler(0, 0, 90);
            var body = truck.gameObject.AddComponent<BoxCollider>();   // one box: nobody walks or drives through it
            body.center = new Vector3(0, 1.3f, .2f); body.size = new Vector3(2.4f, 2.6f, 6.4f);
            return truck.gameObject;
        }

        private static Transform Part(Transform truck, PrimitiveType type, Vector3 at, Vector3 size, Material material)
        {
            GameObject part = CityGeometry.Shape("Truk antre", type, Vector3.zero, size, material, false);
            part.isStatic = false; part.transform.SetParent(truck, false); part.transform.localPosition = at;
            return part.transform;
        }
    }
}
