using UnityEngine;
using Steel.Services;

namespace Steel.Editor
{
    internal static class CityBoundsBuilder
    {
        internal static void Create(CityLayout layout)
        {
            void Wall(string name, Vector3 center, Vector3 size)
            {
                var go = new GameObject(name); go.transform.SetParent(CityGeometry.Root);
                go.transform.position = center; go.AddComponent<BoxCollider>().size = size; go.isStatic = true;
            }
            float midZ = (layout.BoundaryMinZ + layout.BoundaryMaxZ) / 2;
            float depth = layout.BoundaryMaxZ - layout.BoundaryMinZ + 2;
            Wall("Batas selatan", new Vector3(0, 7, layout.BoundaryMinZ), new Vector3(layout.BoundaryHalfWidth * 2 + 2, 16, 1));
            Wall("Batas utara", new Vector3(0, 7, layout.BoundaryMaxZ), new Vector3(layout.BoundaryHalfWidth * 2 + 2, 16, 1));
            Wall("Batas barat", new Vector3(-layout.BoundaryHalfWidth, 7, midZ), new Vector3(1, 16, depth));
            Wall("Batas timur", new Vector3(layout.BoundaryHalfWidth, 7, midZ), new Vector3(1, 16, depth));
        }
    }
}
