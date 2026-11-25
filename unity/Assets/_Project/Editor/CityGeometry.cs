using UnityEngine;

namespace Steel.Editor
{
    internal static class CityGeometry
    {
        internal static Transform Root;
        internal static GameObject Shape(string name, PrimitiveType type, Vector3 point, Vector3 size,
            Material material, bool solid = true)
        {
            var go = GameObject.CreatePrimitive(type); go.name = name; go.transform.SetParent(Root, false);
            go.transform.position = point; go.transform.localScale = size;
            go.GetComponent<Renderer>().sharedMaterial = material; go.isStatic = true;
            if (!solid) Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }
        internal static GameObject Box(string name, Vector3 point, Vector3 size, Material material, bool solid = true) =>
            Shape(name, PrimitiveType.Cube, point, size, material, solid);
        internal static void Sign(string text, Vector3 point, float size, Color color)
        {
            var go = new GameObject(text); go.transform.SetParent(Root, false); go.transform.position = point;
            var mesh = go.AddComponent<TextMesh>(); mesh.text = text; mesh.fontSize = 96; mesh.characterSize = size * .22f;
            mesh.anchor = TextAnchor.MiddleCenter; mesh.alignment = TextAlignment.Center; mesh.color = color;
        }
    }
}
