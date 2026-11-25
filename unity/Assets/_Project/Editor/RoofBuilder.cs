using UnityEditor;
using UnityEngine;

namespace Steel.Editor
{
    internal static class RoofBuilder
    {
        internal static void Create(Vector3 position, float width, float depth, float rise)
        {
            const string path = "Assets/_Project/Generated/GabledRoof.asset";
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (mesh == null)
            {
                var a = new Vector3(-.5f, 0, -.5f); var b = new Vector3(0, 1, -.5f);
                var c = new Vector3(.5f, 0, -.5f); var d = new Vector3(-.5f, 0, .5f);
                var e = new Vector3(0, 1, .5f); var f = new Vector3(.5f, 0, .5f);
                mesh = new Mesh { name = "Industrial gabled roof", vertices = new[] { a,b,c,d,e,f,a,b,d,e,b,c,e,f },
                    triangles = new[] {0,1,2,3,5,4,6,8,7,7,8,9,10,12,11,11,12,13},
                    uv = new[] {Vector2.zero,Vector2.up,Vector2.one,Vector2.zero,Vector2.up,Vector2.one,
                        Vector2.zero,Vector2.up,Vector2.right,Vector2.one,Vector2.zero,Vector2.up,Vector2.right,Vector2.one} };
                mesh.RecalculateNormals(); mesh.RecalculateTangents(); AssetDatabase.CreateAsset(mesh, path);
            }
            var go = new GameObject("Atap pelana / overhang", typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.SetParent(CityGeometry.Root); go.transform.position = position;
            go.transform.localScale = new Vector3(width, rise, depth);
            go.GetComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshCollider>().sharedMesh = mesh;
            var mat = CityMaterials.Surface("corrugated_iron", new Vector2(6, 3)); mat.SetFloat("_Cull", 0);
            go.GetComponent<MeshRenderer>().sharedMaterial = mat; go.isStatic = true;
        }
    }
}
