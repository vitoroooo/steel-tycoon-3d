using UnityEngine;

namespace Steel.Presentation.Vehicles
{
    /// <summary>Collision queries for a kinematic vehicle body, shared by every drivable vehicle.</summary>
    internal static class MotorPhysics
    {
        private const float Skin = .08f, ProbeAbove = 1.2f, ProbeBelow = 4;

        private static Vector3 HalfExtents(BoxCollider box) =>
            Vector3.Scale(box.size * .5f, box.transform.lossyScale);

        private static Vector3 Center(Rigidbody body, BoxCollider box, Quaternion rotation) =>
            body.position + rotation * Vector3.Scale(box.center, box.transform.lossyScale);

        /// <summary>The height a vehicle rides at: the mean ground under its front and rear axle, plus clearance.</summary>
        /// <param name="reach">Metres from the body origin to each axle along forward.</param>
        internal static Vector3 OnGround(Vector3 point, Vector3 forward, float reach, float clearance)
        {
            float front = Surface(point + forward * reach, point.y), rear = Surface(point - forward * reach, point.y);
            point.y = (front + rear) / 2 + clearance;
            return point;
        }

        /// <summary>Probes from just above the body, so roofs, canopies and wires overhead are never the ground.</summary>
        private static float Surface(Vector3 point, float fallback) =>
            Physics.Raycast(point + Vector3.up * ProbeAbove, Vector3.down, out var ground, ProbeAbove + ProbeBelow,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) ? ground.point.y : fallback;

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
