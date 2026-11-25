using UnityEngine;

namespace Steel.Editor
{
    internal static class WorkerBounds
    {
        internal static Bounds Get(GameObject model)
        {
            Bounds bounds = default; bool first = true;
            foreach (var renderer in model.GetComponentsInChildren<SkinnedMeshRenderer>())
                foreach (Vector3 vertex in renderer.sharedMesh.vertices)
                {
                    Vector3 point = renderer.transform.TransformPoint(vertex);
                    if (first) { bounds = new Bounds(point, Vector3.zero); first = false; }
                    else bounds.Encapsulate(point);
                }
            return bounds;
        }
    }
}
