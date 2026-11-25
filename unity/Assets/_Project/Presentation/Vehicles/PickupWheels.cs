using UnityEngine;
using Steel.Services;
using Steel.Presentation.Input;

namespace Steel.Presentation.Vehicles
{
    public sealed class PickupWheels : MonoBehaviour
    {
        public Transform FrontLeft, FrontRight, Rear;
        public float Radius;
        private GameHost host;
        private SliceInput input;
        private float roll;
        private void Start() { host = FindObjectOfType<GameHost>(); input = FindObjectOfType<SliceInput>(); }
        private void LateUpdate()
        {
            roll = (roll + host.Game.Vehicle.Speed * Time.deltaTime / Radius * Mathf.Rad2Deg) % 360;
            float steer = host.Game.Vehicle.Occupied ? input.Move.x * 25 : 0;
            FrontLeft.localRotation = FrontRight.localRotation =
                Quaternion.Euler(0, steer, 0) * Quaternion.Euler(roll, 0, 0);
            Rear.localRotation = Quaternion.Euler(roll, 0, 0);
        }
    }
}
