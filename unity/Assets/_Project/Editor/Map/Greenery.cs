using UnityEngine;
using Steel.Editor.Buildings;
using Steel.Editor.Models;

namespace Steel.Editor.Map
{
    /// <summary>Banana clumps in the leftover corners of yards, where nobody bothered to pave.</summary>
    internal static class Greenery
    {
        /// <summary>x, z, yaw and scale of every clump.</summary>
        private static readonly Vector4[] Bananas =
        {
            // kampung back yards, against the western fence
            new Vector4(-52.2f, 52.5f, 20, 1), new Vector4(-51.9f, 38.4f, 140, 1.15f), new Vector4(-52.3f, 24.6f, 250, .9f),
            new Vector4(-52, 10.4f, 80, 1.1f), new Vector4(-52.2f, -3.2f, 310, 1),
            // the open yard between the lapak and the Pasar Lama alley
            new Vector4(-27.5f, 19.5f, 60, 1.1f), new Vector4(-29, 17.6f, 200, .85f), new Vector4(-12.5f, 19.8f, 120, 1),
            // behind the northern row and around the factory
            new Vector4(24, 41, 30, 1.1f), new Vector4(25.2f, 53.5f, 170, 1), new Vector4(38.5f, 16, 280, 1.05f),
            new Vector4(44, 6.5f, 100, .9f),
        };

        internal static void Create()
        {
            foreach (Vector4 clump in Bananas)
            {
                var plant = AuthoredModel.Create("Nature/Pisang", YardMaterials.Banana);
                plant.name = "Pohon pisang";
                plant.transform.SetParent(CityGeometry.Root, false);
                plant.transform.SetPositionAndRotation(new Vector3(clump.x, -.1f, clump.y), Quaternion.Euler(0, clump.z, 0));
                plant.transform.localScale = Vector3.one * clump.w;
                foreach (Transform part in plant.GetComponentsInChildren<Transform>()) part.gameObject.isStatic = true;
                var trunk = plant.AddComponent<CapsuleCollider>();   // only the stem blocks; leaves brush past
                trunk.center = Vector3.up * 1.15f; trunk.radius = .14f; trunk.height = 2.3f;
            }
        }
    }
}
