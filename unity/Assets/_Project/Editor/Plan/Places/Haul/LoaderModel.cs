using UnityEngine;

namespace Steel.Editor.Plan.Places.Haul
{
    /// <summary>A small cartoon wheel loader from a handful of flat-coloured shapes; forward is +Z.</summary>
    internal static class LoaderModel
    {
        internal static Transform Create(Transform parent)
        {
            var loader = new GameObject("Loader").transform;
            loader.SetParent(parent, false);
            Material yellow = CityMaterials.Plain("Loader kuning", new Color(.95f, .71f, .2f)),
                navy = CityMaterials.Plain("Loader navy", new Color(.12f, .18f, .29f)),
                tyre = CityMaterials.Plain("Loader ban", new Color(.1f, .11f, .14f));
            Part(loader, PrimitiveType.Cube, new Vector3(0, .95f, -.3f), new Vector3(1.5f, .8f, 2.6f), yellow);
            Part(loader, PrimitiveType.Cube, new Vector3(0, 1.85f, -.6f), new Vector3(1.2f, 1.1f, 1.2f), navy);
            Part(loader, PrimitiveType.Cube, new Vector3(0, 1.1f, 1.5f), new Vector3(.3f, .3f, 1.6f), yellow);
            Part(loader, PrimitiveType.Cube, new Vector3(0, .7f, 2.4f), new Vector3(1.9f, .8f, .7f), navy);
            foreach (int side in new[] { -1, 1 })
                foreach (float z in new[] { -1.1f, .6f })
                    Part(loader, PrimitiveType.Cylinder, new Vector3(side * .85f, .55f, z), new Vector3(1.1f, .2f, 1.1f), tyre)
                        .localRotation = Quaternion.Euler(0, 0, 90);
            var body = loader.gameObject.AddComponent<BoxCollider>();   // one box keeps people and trucks out of it
            body.center = new Vector3(0, 1.2f, .3f); body.size = new Vector3(1.9f, 2.4f, 4.4f);
            return loader;
        }

        private static Transform Part(Transform loader, PrimitiveType type, Vector3 at, Vector3 size, Material material)
        {
            GameObject part = CityGeometry.Shape("Loader", type, Vector3.zero, size, material, false);
            part.isStatic = false; part.transform.SetParent(loader, false); part.transform.localPosition = at;
            return part.transform;
        }
    }
}
