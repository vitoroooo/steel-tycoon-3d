using UnityEngine;
using Steel.Presentation.Vehicles;
using Steel.Presentation.World;
using Steel.Services;

namespace Steel.Editor.Vehicles
{
    /// <summary>What every drivable vehicle needs on its root: body, driver, camera hand-over, recovery.</summary>
    internal static class MotorRig
    {
        private const int IgnoreRaycastLayer = 2;
        /// <summary>Vehicles for sale face the street south of the dealer lot.</summary>
        private const float DealerYaw = 180;

        /// <summary>Stands the vehicle on its configured spot and returns its driver.</summary>
        internal static MotorDriver Attach(GameObject root, string id, Vector3 boxCenter, Vector3 boxSize,
            params Vector3[] exitPoints)
        {
            var source = new ResourceConfigSource();
            VehicleView view = new LocalGameService(source).VehicleOf(id);
            var layout = new CityLayout(source);
            root.transform.SetPositionAndRotation(new Vector3(view.X, 0, view.Z), Quaternion.Euler(0, DealerYaw, 0));
            foreach (Transform part in root.GetComponentsInChildren<Transform>())
            {
                part.gameObject.layer = IgnoreRaycastLayer;
                part.gameObject.isStatic = false;
            }
            var box = root.AddComponent<BoxCollider>();
            box.center = boxCenter; box.size = boxSize;
            root.AddComponent<Rigidbody>().isKinematic = true;
            root.AddComponent<FallRecovery>().SpawnOffset =
                new Vector3(view.X - layout.Spawn.X, 0, view.Z - layout.Spawn.Z);
            var driver = root.AddComponent<MotorDriver>();
            driver.VehicleId = id; driver.ExitPoints = exitPoints;
            root.AddComponent<DrivenCamera>().Driver = driver;
            return driver;
        }

        internal static Transform Anchor(string name, Transform parent, Vector3 local)
        {
            var anchor = new GameObject(name).transform;
            anchor.SetParent(parent, false);
            anchor.localPosition = local;
            return anchor;
        }
    }
}
