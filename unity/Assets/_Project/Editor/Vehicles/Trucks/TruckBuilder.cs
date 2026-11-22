using System.Collections.Generic;
using UnityEngine;
using Steel.Editor.Characters.Toon;
using Steel.Editor.Models;
using Steel.Presentation.Vehicles;
using Steel.Presentation.Vehicles.Towing;

namespace Steel.Editor.Vehicles.Trucks
{
    /// <summary>Stands one heavy truck in the city: body, rolling wheels, steered front axle, hidden driver.</summary>
    internal static class TruckBuilder
    {
        private const float DoorX = 1.9f;

        internal static Transform Create(TruckSpec spec, Transform player, float yaw)
        {
            GameObject root = AuthoredModel.Create("Vehicles/" + spec.Model, _ => ToonCast.Material);
            root.name = spec.Name;
            var rolling = new List<Transform>();
            foreach (Transform part in root.transform)
                if (part.name.Contains("Wheel_")) rolling.Add(part);
            var wheels = root.AddComponent<MotorWheels>();
            wheels.Radius = TruckSpec.WheelRadius; wheels.SteerDegrees = 28;
            wheels.Steered = new[] { Kingpin(root.transform.Find("Wheel_1L")), Kingpin(root.transform.Find("Wheel_1R")) };
            wheels.Rolling = rolling.ToArray();
            MotorDriver driver = MotorRig.Attach(root, spec.Id, yaw, new Vector3(0, TruckSpec.Height / 2, 0),
                new Vector3(TruckSpec.Width, TruckSpec.Height, spec.Length),
                new Vector3(-DoorX, 0, spec.DoorZ), new Vector3(DoorX, 0, spec.DoorZ), new Vector3(0, 0, spec.Length / 2 + 1.5f));
            driver.AxleReach = spec.AxleReach;
            wheels.Driver = driver;
            var seat = root.AddComponent<RiderSeat>();
            seat.Driver = driver; seat.Enclosed = true; seat.RiderHome = player;
            seat.Rider = player.GetComponent<Steel.Presentation.Player.PlayerMover>().Visual;
            if (spec.Towing) Couple(root, driver, rolling);
            MotorRig.Suspend(root, driver);
            return root.transform;
        }

        /// <summary>Hangs the deck and its wheels on the kingpin, so the deck swings behind the tractor.</summary>
        private static void Couple(GameObject root, MotorDriver driver, List<Transform> rolling)
        {
            Transform deck = root.transform.Find("Deck");
            foreach (Transform wheel in rolling)
                if (wheel.name.StartsWith("DeckWheel_")) wheel.SetParent(deck, true);
            var solid = deck.gameObject.AddComponent<BoxCollider>();   // people walk around the deck, not through it
            solid.center = new Vector3(0, 0, TruckSpec.DeckNose - TruckSpec.DeckLength / 2);
            solid.size = new Vector3(TruckSpec.Width, 1.6f, TruckSpec.DeckLength);
            var hitch = root.AddComponent<TrailerHitch>();
            hitch.Driver = driver; hitch.Deck = deck; hitch.Reach = TruckSpec.DeckReach;
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
