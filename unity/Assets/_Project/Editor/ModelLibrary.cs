using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Steel.Editor
{
    internal static class ModelLibrary
    {
        internal static GameObject Place(string name, Vector3 position, float height, float yaw = 0,
            Transform parent = null, string part = null)
        {
            string path = name == "Pickup" ? "Assets/ThirdParty/Quaternius/Pickup.glb" :
                "Assets/ThirdParty/PolyHavenModels/" + name + "/" + name + ".gltf";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) throw new InvalidOperationException("Model not imported: " + path);
            GameObject source = part == null ? prefab :
                prefab.GetComponentsInChildren<MeshRenderer>(true).First(r => r.name == part).gameObject;
            var go = UnityEngine.Object.Instantiate(source); go.name = name;
            go.transform.SetParent(parent == null ? CityGeometry.Root : parent, false);
            go.transform.localRotation = Quaternion.Euler(0, yaw, 0); go.transform.localPosition = Vector3.zero;
            Bounds bounds = BoundsOf(go); go.transform.localScale *= height / Mathf.Max(.001f, bounds.size.y);
            bounds = BoundsOf(go);
            go.transform.position += position - new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
            foreach (Transform t in go.GetComponentsInChildren<Transform>()) t.gameObject.isStatic = true;
            foreach (var filter in go.GetComponentsInChildren<MeshFilter>())
                filter.gameObject.AddComponent<MeshCollider>().sharedMesh = filter.sharedMesh;
            return go;
        }
        internal static Bounds BoundsOf(GameObject go)
        {
            var renderers = go.GetComponentsInChildren<Renderer>(); var b = renderers[0].bounds;
            foreach (var renderer in renderers) b.Encapsulate(renderer.bounds); return b;
        }
    }
}
