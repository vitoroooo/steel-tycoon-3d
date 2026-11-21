using UnityEngine;
using Steel.Presentation.Vehicles;
using Steel.Services;

namespace Steel.Editor.Vehicles
{
    /// <summary>The pikap bekas of PRD 5.1: 1 ton, for sale at the dealer until the player can afford it.</summary>
    internal static class PikapBuilder
    {
        internal static Transform Create(Transform player)
        {
            var root = new GameObject("Pikap bekas");
            ModelLibrary.Place("Pickup", Vector3.up * .2f, 1.9f, 0, root.transform);
            MotorWheels wheels = PikapWheels.Create(root);
            MotorDriver driver = MotorRig.Attach(root, FleetCatalog.Pikap, Vector3.up,
                new Vector3(1.85f, 1.65f, 4.4f),
                new Vector3(-1.7f, 0, 0), new Vector3(1.7f, 0, 0), new Vector3(0, 0, -3));
            wheels.Driver = driver;
            var seat = root.AddComponent<RiderSeat>();
            seat.Driver = driver; seat.Enclosed = true; seat.RiderHome = player;
            seat.Rider = player.GetComponent<Steel.Presentation.Player.PlayerMover>().Visual;
            return root.transform;
        }
    }
}
