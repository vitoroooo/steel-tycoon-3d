using UnityEngine;

namespace Steel.PlayTests.Support
{
    /// <summary>Measures how far something stands above the surface right below it.</summary>
    internal static class GroundProbe
    {
        /// <summary>Metres between the point and the ground under it; positive means floating.</summary>
        internal static float Gap(Vector3 point)
        {
            bool found = Physics.Raycast(point + Vector3.up, Vector3.down, out RaycastHit ground, 5,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            return found ? point.y - ground.point.y : float.PositiveInfinity;
        }

        /// <summary>Names whatever stands within three metres ahead of a vehicle, for failure messages.</summary>
        internal static string Ahead(Transform vehicle) =>
            Physics.SphereCast(vehicle.position + Vector3.up * .9f, .5f, vehicle.forward, out RaycastHit hit, 3,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
                ? hit.collider.name + " at " + hit.collider.bounds.center + " size " + hit.collider.bounds.size
                : "nothing";
    }
}
