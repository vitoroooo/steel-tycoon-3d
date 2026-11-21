using UnityEngine;
using Steel.Presentation.Player;

namespace Steel.Presentation.Vehicles
{
    /// <summary>Hands the orbit camera to this vehicle while it is driven, and back afterwards.</summary>
    public sealed class DrivenCamera : MonoBehaviour
    {
        public MotorDriver Driver;
        private OrbitCamera orbit;

        private void Start() => orbit = FindObjectOfType<OrbitCamera>();

        private void Update()
        {
            if (Driver.View.Occupied) orbit.VehicleTarget = transform;
            else if (orbit.VehicleTarget == transform) orbit.VehicleTarget = null;
        }
    }
}
