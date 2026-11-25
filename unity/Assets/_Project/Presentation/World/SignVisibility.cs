using UnityEngine;

namespace Steel.Presentation.World
{
    [RequireComponent(typeof(TextMesh))]
    public sealed class SignVisibility : MonoBehaviour
    {
        private Renderer mesh;
        private Camera eye;
        private float nextCheck;
        private void Awake() => mesh = GetComponent<Renderer>();
        private void LateUpdate()
        {
            if (Time.unscaledTime < nextCheck) return;
            nextCheck = Time.unscaledTime + .075f;
            if (!eye && !(eye = Camera.main)) return;
            Vector3 p = eye.WorldToViewportPoint(transform.position);
            Vector3 span = eye.WorldToViewportPoint(transform.position + mesh.bounds.extents);
            float halfWidth = Mathf.Abs(span.x - p.x), halfHeight = Mathf.Abs(span.y - p.y);
            var rect = Rect.MinMaxRect(p.x - halfWidth, p.y - halfHeight, p.x + halfWidth, p.y + halfHeight);
            bool overlap = rect.Overlaps(new Rect(0, .76f, .44f, .24f)) ||
                rect.Overlaps(new Rect(.70f, .76f, .30f, .24f)) || rect.Overlaps(new Rect(0, 0, 1, .28f));
            mesh.enabled = p.z > 3 && !overlap;
        }
    }
}
