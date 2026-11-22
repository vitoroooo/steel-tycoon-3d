using UnityEngine;

namespace Steel.Editor.Vehicles
{
    /// <summary>Where a parked vehicle's wheels meet whatever was built under it: road, pavement or a lot floor.</summary>
    internal static class ParkingSpot
    {
        /// <summary>Above any kerb or lot floor, below canopies and roofs.</summary>
        private const float ProbeHeight = 1.2f;

        /// <summary>The point on the surface at this map position. Build the floor before parking on it.</summary>
        internal static Vector3 On(float x, float z)
        {
            Physics.SyncTransforms();
            var above = new Vector3(x, ProbeHeight, z);
            bool found = Physics.Raycast(above, Vector3.down, out RaycastHit floor, ProbeHeight * 2,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            return new Vector3(x, found ? floor.point.y : 0, z);
        }
    }
}
