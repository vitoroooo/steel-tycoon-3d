using UnityEngine;

namespace Steel.Presentation.Player
{
    public static class CameraCollision
    {
        private static float Clearance(Vector3 focus, Vector3 offset)
        {
            float length = offset.magnitude;
            if (Physics.SphereCast(focus, .35f, offset.normalized, out var hit, length,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                return Mathf.Max(.05f, hit.distance - .2f);
            return length;
        }
        public static Vector3 Resolve(Vector3 focus, Vector3 desired)
        {
            Vector3 best = desired.normalized * Clearance(focus, desired);
            if (best.magnitude < desired.magnitude * .85f)
                foreach (float angle in new[] { 22f, -22f, 45f, -45f, 90f, -90f, 135f, -135f, 180f })
                {
                    Vector3 candidate = Quaternion.Euler(0, angle, 0) * desired;
                    candidate = candidate.normalized * Clearance(focus, candidate);
                    if (candidate.magnitude > best.magnitude + .2f) best = candidate;
                }
            if (best.magnitude < desired.magnitude * .5f)
                foreach (float angle in new[] { 0f, 45f, -45f, 90f, -90f, 180f })
                {
                    Vector3 low = new Vector3(desired.x, .5f, desired.z);
                    Vector3 candidate = Quaternion.Euler(0, angle, 0) * low;
                    candidate = candidate.normalized * Clearance(focus, candidate);
                    if (candidate.magnitude > best.magnitude + .2f) best = candidate;
                }
            return focus + best;
        }
    }
}
