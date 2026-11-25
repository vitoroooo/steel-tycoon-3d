using UnityEngine;

namespace Steel.Presentation.Player
{
    public static class CameraCollision
    {
        private const int Mask = Physics.DefaultRaycastLayers;
        public static Vector3 AlongSwipe(Vector3 focus, Vector3 desired, ref float boom, float dt)
        {
            float length = desired.magnitude;
            if (Physics.SphereCast(focus, .34f, desired.normalized, out var hit, length, Mask, QueryTriggerInteraction.Ignore))
                length = Mathf.Max(.08f, hit.distance - .08f);
            if (Physics.Raycast(focus, desired.normalized, out var wall, length, Mask, QueryTriggerInteraction.Ignore))
                length = Mathf.Max(.08f, wall.distance - .08f);
            boom = boom <= 0 ? length : Mathf.Min(length, boom + 12 * dt);
            return focus + desired.normalized * boom;
        }
        private static float Clearance(Vector3 focus, Vector3 offset)
        {
            float length = offset.magnitude, clear = length; Vector3 direction = offset.normalized;
            if (Physics.SphereCast(focus, 1.1f, direction, out var wide, length, Mask, QueryTriggerInteraction.Ignore))
                clear = Mathf.Max(.05f, wide.distance - .2f);
            // The wide sweep skips colliders it already overlaps, so a thin sweep catches walls beside the focus.
            if (Physics.SphereCast(focus, .2f, direction, out var thin, length, Mask, QueryTriggerInteraction.Ignore))
                clear = Mathf.Min(clear, Mathf.Max(.05f, thin.distance - .1f));
            return clear;
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
