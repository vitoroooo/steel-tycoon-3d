using UnityEngine;
using Steel.Editor.Characters.Toon;
using Steel.Editor.Models;
using Steel.Presentation.Vehicles;
using Steel.Services;

namespace Steel.Editor.Vehicles
{
    /// <summary>Places the starter gerobak next to the player and wires it to its pusher.</summary>
    internal static class GerobakBuilder
    {
        private const int IgnoreRaycastLayer = 2;
        // Measurements of tools/blender/kartun/gerobak, in the model's own space (forward is +Z).
        private const float GripSpacing = .34f, GripHeight = 1.03f, GripBack = -1.17f, WristPitch = 25;

        internal static Transform Create(Transform player)
        {
            CartView cart = new LocalGameService(new ResourceConfigSource()).Cart;
            GameObject root = AuthoredModel.Create("Vehicles/GerobakKartun", _ => ToonCast.Material);
            root.name = "Gerobak dorong";
            root.transform.position = new Vector3(cart.X, player.position.y, cart.Z);
            foreach (Transform part in root.GetComponentsInChildren<Transform>())
                part.gameObject.layer = IgnoreRaycastLayer;
            var box = root.AddComponent<BoxCollider>();
            box.center = new Vector3(0, .55f, -.3f);
            box.size = new Vector3(1.2f, 1, 1.95f);
            var pusher = root.AddComponent<CartPusher>();
            pusher.Player = player;
            pusher.PlayerVisual = player.GetComponent<Steel.Presentation.Player.PlayerMover>().Visual;
            pusher.WheelLeft = root.transform.Find("Wheel_L");
            pusher.WheelRight = root.transform.Find("Wheel_R");
            pusher.GripLeft = Grip(root.transform, -1);
            pusher.GripRight = Grip(root.transform, 1);
            return root.transform;
        }

        /// <summary>Where a wrist rests when the palm lies on the rubber grip of the push bar.</summary>
        private static Transform Grip(Transform cart, float side)
        {
            var grip = new GameObject(side < 0 ? "Grip_L" : "Grip_R").transform;
            grip.SetParent(cart, false);
            grip.localPosition = new Vector3(side * GripSpacing, GripHeight, GripBack);
            grip.localRotation = Quaternion.Euler(WristPitch, 0, 0);
            return grip;
        }
    }
}
