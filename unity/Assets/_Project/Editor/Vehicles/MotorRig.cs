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
        private const float KerbClearance = .34f;
        /// <summary>Vehicles for sale face west: straight down the driveway to the main road.</summary>
        internal const float DealerYaw = 270;

        /// <summary>Stands the vehicle on its configured spot and returns its driver.</summary>
        internal static MotorDriver Attach(GameObject root, string id, Vector3 boxCenter, Vector3 boxSize,
            params Vector3[] exitPoints) => Attach(root, id, DealerYaw, boxCenter, boxSize, exitPoints);

        /// <summary>The same, facing yaw degrees from north.</summary>
        internal static MotorDriver Attach(GameObject root, string id, float yaw, Vector3 boxCenter, Vector3 boxSize,
            params Vector3[] exitPoints)
        {
            var source = new ResourceConfigSource();
            VehicleView view = new LocalGameService(source).VehicleOf(id);
            var layout = new CityLayout(source);
            root.transform.SetPositionAndRotation(ParkingSpot.On(view.X, view.Z), Quaternion.Euler(0, yaw, 0));
            foreach (Transform part in root.GetComponentsInChildren<Transform>())
            {
                part.gameObject.layer = IgnoreRaycastLayer;
                part.gameObject.isStatic = false;
            }
            var box = root.AddComponent<BoxCollider>();
            // The body starts above kerb height: wheels climb a low step, only walls and props stop the vehicle.
            float top = boxCenter.y + boxSize.y / 2;
            box.center = new Vector3(boxCenter.x, (top + KerbClearance) / 2, boxCenter.z);
            box.size = new Vector3(boxSize.x, top - KerbClearance, boxSize.z);
            root.AddComponent<Rigidbody>().isKinematic = true;
            root.AddComponent<FallRecovery>().SpawnOffset =
                new Vector3(view.X - layout.Spawn.X, 0, view.Z - layout.Spawn.Z);
            var driver = root.AddComponent<MotorDriver>();
            driver.VehicleId = id; driver.ExitPoints = exitPoints;
            root.AddComponent<DrivenCamera>().Driver = driver;
            return driver;
        }

        /// <summary>Hangs everything built so far on a chassis that rocks on its springs. Call last.</summary>
        internal static void Suspend(GameObject root, MotorDriver driver)
        {
            var chassis = new GameObject("Chassis").transform;
            chassis.gameObject.layer = IgnoreRaycastLayer;
            chassis.SetParent(root.transform, false);
            for (int i = root.transform.childCount - 1; i >= 0; i--)
                if (root.transform.GetChild(i) != chassis) root.transform.GetChild(i).SetParent(chassis, true);
            var sway = root.AddComponent<BodySway>();
            sway.Driver = driver; sway.Chassis = chassis;
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
