using UnityEngine;

namespace Steel.Presentation.Vehicles
{
    /// <summary>Turns the steered parts with the input and rolls every wheel with the road speed.</summary>
    public sealed class MotorWheels : MonoBehaviour
    {
        public MotorDriver Driver;
        /// <summary>Parts that swing with the steering: wheel pivots, or a whole fork and handlebar.</summary>
        public Transform[] Steered;
        /// <summary>Wheels, each spinning around its own local X axis.</summary>
        public Transform[] Rolling;
        public Vector3 SteerAxis = Vector3.up;
        public float SteerDegrees = 25, Radius = .3f;
        private const float SteerRate = 140;
        private float roll, steer;

        private void LateUpdate()
        {
            roll = (roll + Driver.View.Speed * Time.deltaTime / Radius * Mathf.Rad2Deg) % 360;
            steer = Mathf.MoveTowards(steer, Driver.Steer * SteerDegrees, SteerRate * Time.deltaTime);
            Quaternion turned = Quaternion.AngleAxis(steer, SteerAxis.normalized);
            foreach (Transform part in Steered) part.localRotation = turned;
            Quaternion rolled = Quaternion.Euler(roll, 0, 0);
            foreach (Transform wheel in Rolling) wheel.localRotation = rolled;
        }
    }
}
