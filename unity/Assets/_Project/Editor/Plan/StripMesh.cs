using System.Collections.Generic;
using UnityEngine;

namespace Steel.Editor.Plan
{
    /// <summary>Turns a list of quads (four corners each, clockwise from the visible side) into a flat-shaded mesh.</summary>
    internal static class StripMesh
    {
        internal static Mesh Create(string name, StripBatch batch)
        {
            List<Vector3> corners = batch.Corners;
            var normals = new Vector3[corners.Count];
            var triangles = new int[corners.Count / 4 * 6];
            for (int first = 0, t = 0; first < corners.Count; first += 4)
            {
                // The far pair of corners gives the facing even when a fan's near pair meets in one point.
                Vector3 normal = Vector3.Cross(corners[first + 2] - corners[first], corners[first + 3] - corners[first]).normalized;
                for (int i = 0; i < 4; i++) normals[first + i] = normal;
                triangles[t++] = first; triangles[t++] = first + 1; triangles[t++] = first + 2;
                triangles[t++] = first; triangles[t++] = first + 2; triangles[t++] = first + 3;
            }
            var mesh = new Mesh { name = name };
            if (corners.Count > 65000) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(corners); mesh.SetNormals(normals); mesh.SetUVs(0, batch.Tints); mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>The ground footprint of every quad, for callers that must know what a strip covers.</summary>
        internal static IEnumerable<Bounds> Footprints(Mesh mesh)
        {
            Vector3[] corners = mesh.vertices;
            for (int first = 0; first + 3 < corners.Length; first += 4)
            {
                var bounds = new Bounds(corners[first], Vector3.zero);
                for (int i = 1; i < 4; i++) bounds.Encapsulate(corners[first + i]);
                yield return bounds;
            }
        }
    }
}
