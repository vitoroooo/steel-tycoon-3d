using System.Linq;
using UnityEngine;
using Steel.Editor.Characters.Toon;
using Steel.Editor.Models;
using Steel.Presentation.Vehicles;
using Steel.Services;

namespace Steel.Editor.Vehicles
{
    /// <summary>The pikap bekas of PRD 5.1: 1 ton, for sale at the dealer until the player can afford it.</summary>
    internal static class PikapBuilder
    {
        // Measurements of tools/blender/kartun/pikap, in the model's own space (forward is +Z).
        private static readonly string[] WheelNames = { "Wheel_FL", "Wheel_FR", "Wheel_RL", "Wheel_RR" };
        private static readonly Vector3 BodyCentre = new Vector3(0, .95f, -.05f), BodySize = new Vector3(1.8f, 1.9f, 4.2f);
        private const float WheelRadius = .36f, HalfWheelbase = 1.2f, DoorX = 1.5f, DoorZ = 1.3f;

        internal static Transform Create(Transform player)
        {
            GameObject root = AuthoredModel.Create("Vehicles/PikapKartun", _ => ToonCast.Material);
            root.name = "Pikap bekas";
            Transform[] rolling = WheelNames.Select(name => root.transform.Find(name)).ToArray();
            var wheels = root.AddComponent<MotorWheels>();
            wheels.Radius = WheelRadius;
            wheels.Steered = new[] { Kingpin(rolling[0]), Kingpin(rolling[1]) };
            wheels.Rolling = rolling;
            MotorDriver driver = MotorRig.Attach(root, FleetCatalog.Pikap, BodyCentre, BodySize,
                new Vector3(-DoorX, 0, DoorZ), new Vector3(DoorX, 0, DoorZ), new Vector3(0, 0, -3));
            driver.AxleReach = HalfWheelbase;
            wheels.Driver = driver;
            var seat = root.AddComponent<RiderSeat>();
            seat.Driver = driver; seat.Enclosed = true; seat.RiderHome = player;
            seat.Rider = player.GetComponent<Steel.Presentation.Player.PlayerMover>().Visual;
            MotorRig.Suspend(root, driver);
            return root.transform;
        }

        /// <summary>A pivot at the wheel's hub: it swings with the steering while the wheel rolls inside it.</summary>
        private static Transform Kingpin(Transform wheel)
        {
            var pivot = new GameObject(wheel.name + " / kemudi").transform;
            pivot.SetParent(wheel.parent, false);
            pivot.localPosition = wheel.localPosition;
            wheel.SetParent(pivot, true);
            return pivot;
        }
    }
}
