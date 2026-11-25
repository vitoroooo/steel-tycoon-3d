using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steel.Presentation.Vehicles;
using Steel.Services;

namespace Steel.PlayTests.Support
{
    /// <summary>Loads the city with enough cash to buy a vehicle, so tests can get straight to driving.</summary>
    internal static class OwnedFleet
    {
        internal static IEnumerator LoadWealthyCity()
        {
            GameHost.Source = new OverrideConfigSource(new ResourceConfigSource(), "City", "startCash", 200000000);
            yield return SceneManager.LoadSceneAsync("City");
            GameHost.Source = null;
            yield return null;
        }

        internal static MotorDriver Driver(string id) =>
            Object.FindObjectsOfType<MotorDriver>().First(d => d.VehicleId == id);

        /// <summary>Teleports the player object, so the next frame reports this spot to the game.</summary>
        internal static void PlacePlayer(GameHost host, float x, float z)
        {
            var controller = Object.FindObjectOfType<CharacterController>();
            controller.enabled = false;
            controller.transform.position = new Vector3(x, host.Layout.SpawnY, z);
            controller.enabled = true;
            host.Game.SetPosition(x, z);
        }

        /// <summary>Stands the player at the vehicle and pays for it.</summary>
        internal static MotorDriver Buy(GameHost host, string id)
        {
            VehicleView vehicle = host.Game.VehicleOf(id);
            host.Game.SetPosition(vehicle.X, vehicle.Z);
            host.Game.BuyVehicle(id);
            return Driver(id);
        }
    }
}
