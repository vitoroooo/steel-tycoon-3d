using UnityEngine;

namespace Steel.Presentation.World
{
    [RequireComponent(typeof(TextMesh))]
    public sealed class SignVisibility : MonoBehaviour
    {
        private Renderer mesh;
        private void Awake() => mesh = GetComponent<Renderer>();
        private void LateUpdate()
        {
            var camera = Camera.main;
            if (camera == null) return;
            Vector3 p = camera.WorldToViewportPoint(transform.position);
            Vector3 span = camera.WorldToViewportPoint(transform.position + mesh.bounds.extents);
            float halfWidth = Mathf.Abs(span.x - p.x), halfHeight = Mathf.Abs(span.y - p.y);
            var rect = Rect.MinMaxRect(p.x - halfWidth, p.y - halfHeight, p.x + halfWidth, p.y + halfHeight);
            bool overlap = rect.Overlaps(new Rect(0, .76f, .44f, .24f)) ||
                rect.Overlaps(new Rect(.70f, .76f, .30f, .24f)) || rect.Overlaps(new Rect(0, 0, 1, .28f));
            mesh.enabled = p.z > 3 && !overlap;
        }
    }
}
