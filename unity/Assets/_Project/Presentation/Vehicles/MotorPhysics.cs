using UnityEngine;

namespace Steel.Presentation.Vehicles
{
    /// <summary>Collision queries for a kinematic vehicle body, shared by every drivable vehicle.</summary>
    internal static class MotorPhysics
    {
        private const float Skin = .08f;

        private static Vector3 HalfExtents(BoxCollider box) =>
            Vector3.Scale(box.size * .5f, box.transform.lossyScale);

        private static Vector3 Center(Rigidbody body, BoxCollider box, Quaternion rotation) =>
            body.position + rotation * Vector3.Scale(box.center, box.transform.lossyScale);

        /// <summary>Drops a point onto the surface below it, plus the clearance the body rides at.</summary>
        internal static Vector3 OnGround(Vector3 point, float clearance)
        {
            if (Physics.Raycast(point + Vector3.up * 3, Vector3.down, out var ground, 8, Physics.DefaultRaycastLayers))
                point.y = ground.point.y + clearance;
            return point;
        }

        /// <summary>True when moving by delta would run the body into something solid.</summary>
        internal static bool Blocked(Rigidbody body, BoxCollider box, Quaternion rotation, Vector3 delta) =>
            delta.sqrMagnitude > 0 && Physics.BoxCast(Center(body, box, rotation), HalfExtents(box),
                delta.normalized, out _, rotation, delta.magnitude + Skin,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

        /// <summary>True when the body, turned to rotation, would overlap something that is not itself.</summary>
        internal static bool Overlaps(Rigidbody body, BoxCollider box, Quaternion rotation)
        {
            foreach (var hit in Physics.OverlapBox(Center(body, box, rotation), HalfExtents(box), rotation,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                if (hit.attachedRigidbody != body && !hit.transform.IsChildOf(body.transform)) return true;
            return false;
        }
    }
}
