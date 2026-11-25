using UnityEngine;
using Steel.Editor.Models;
using Steel.Presentation.Vehicles;
using Steel.Services;

namespace Steel.Editor.Vehicles
{
    /// <summary>The motor triseda of PRD 5.1: 300 kg, ridden in the open with both hands on the bar.</summary>
    internal static class TrisedaBuilder
    {
        // Measurements of tools/blender/triseda, in the model's own space (forward is +Z).
        private static readonly Vector3 SteeringAxis = new Vector3(0, .71f, -.32f);
        private static readonly Vector3 HipJoints = new Vector3(0, .98f, .48f);
        private const float GripX = .34f, GripY = 1.13f, GripZ = .98f;
        private const float PegX = .3f, AnkleY = .37f, PegZ = .58f;
        private const float WheelRadius = .27f, SteerDegrees = 25;

        internal static Transform Create(Transform player)
        {
            GameObject root = AuthoredModel.Create("Vehicles/Triseda", TrisedaMaterials.For);
            root.name = "Motor triseda";
            Transform steer = root.transform.Find("Steer"), front = root.transform.Find("Wheel_F");
            front.SetParent(steer, true);   // the FBX keeps it beside the fork; it turns with the bar
            MotorDriver driver = MotorRig.Attach(root, FleetCatalog.Triseda, new Vector3(0, .75f, .05f),
                new Vector3(1.3f, 1.4f, 3.2f),
                new Vector3(-1.1f, 0, .3f), new Vector3(1.1f, 0, .3f), new Vector3(0, 0, 2.4f));
            var wheels = root.AddComponent<MotorWheels>();
            wheels.Driver = driver; wheels.Radius = WheelRadius;
            wheels.SteerAxis = SteeringAxis; wheels.SteerDegrees = SteerDegrees;
            wheels.Steered = new[] { steer };
            wheels.Rolling = new[] { front, root.transform.Find("Wheel_RL"), root.transform.Find("Wheel_RR") };
            Seat(root, steer, driver, player);
            MotorRig.Suspend(root, driver);
            return root.transform;
        }

        private static void Seat(GameObject root, Transform steer, MotorDriver driver, Transform player)
        {
            var seat = root.AddComponent<RiderSeat>();
            seat.Driver = driver; seat.RiderHome = player;
            seat.Rider = player.GetComponent<Steel.Presentation.Player.PlayerMover>().Visual;
            seat.Mount = MotorRig.Anchor("Seat / mount", root.transform, new Vector3(0, 0, HipJoints.z));
            seat.Hips = MotorRig.Anchor("Seat / hips", root.transform, HipJoints);
            seat.FootLeft = MotorRig.Anchor("Seat / foot L", root.transform, new Vector3(-PegX, AnkleY, PegZ));
            seat.FootRight = MotorRig.Anchor("Seat / foot R", root.transform, new Vector3(PegX, AnkleY, PegZ));
            seat.GripLeft = Grip(steer, -1); seat.GripRight = Grip(steer, 1);
        }

        /// <summary>Wrist position over a rubber grip; the anchor hangs under Steer so it turns with the bar.</summary>
        private static Transform Grip(Transform steer, float side)
        {
            Transform grip = MotorRig.Anchor(side < 0 ? "Grip_L" : "Grip_R", steer.parent,
                new Vector3(side * GripX, GripY, GripZ));
            grip.SetParent(steer, true);
            grip.localRotation = Quaternion.Euler(20, 0, 0);
            return grip;
        }
    }
}
