using UnityEditor;
using UnityEngine;

namespace Steel.Editor
{
    internal static class SheetMesh
    {
        internal static Mesh Get()
        {
            const string path = "Assets/_Project/Generated/CorrugatedSheet.asset";
            var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing != null) return existing;
            const int segments = 28;   // seven broad ridges: reads as corrugated without a saw-tooth edge
            var vertices = new Vector3[(segments + 1) * 2]; var uv = new Vector2[vertices.Length];
            var triangles = new int[segments * 6];
            for (int i = 0; i <= segments; i++)
                for (int j = 0; j < 2; j++)
                {
                    float x = (float)i / segments;
                    vertices[i * 2 + j] = new Vector3(x - .5f,
                        Mathf.Sin(x * Mathf.PI * 14) * .022f + Mathf.Pow(x - .5f, 2) * .13f, j - .5f);
                    uv[i * 2 + j] = new Vector2(x, j);
                    if (j == 0 && i < segments)
                    {
                        int t = i * 6, k = i * 2;
                        triangles[t] = k; triangles[t + 1] = k + 1; triangles[t + 2] = k + 2;
                        triangles[t + 3] = k + 1; triangles[t + 4] = k + 3; triangles[t + 5] = k + 2;
                    }
                }
            var mesh = new Mesh { name = "Bent corrugated steel", vertices = vertices, uv = uv, triangles = triangles };
            mesh.RecalculateNormals(); mesh.RecalculateTangents(); AssetDatabase.CreateAsset(mesh, path); return mesh;
        }
        internal static GameObject Place(Vector3 position, Vector3 scale, Quaternion rotation, Transform parent = null)
        {
            var go = new GameObject("Seng bergelombang", typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.SetParent(parent == null ? CityGeometry.Root : parent);
            go.transform.position = position; go.transform.localScale = scale; go.transform.rotation = rotation;
            go.GetComponent<MeshFilter>().sharedMesh = Get();
            go.AddComponent<MeshCollider>().sharedMesh = Get();
            var mat = CityMaterials.Surface("corrugated_iron", Vector2.one); mat.SetFloat("_Cull", 0);
            go.GetComponent<MeshRenderer>().sharedMaterial = mat; go.isStatic = true; return go;
        }
    }
}
