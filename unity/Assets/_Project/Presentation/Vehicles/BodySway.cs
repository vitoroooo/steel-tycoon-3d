using UnityEngine;

namespace Steel.Presentation.Vehicles
{
    /// <summary>Rocks the chassis on its springs: nose up under throttle, dip under braking, roll in corners, engine shake.</summary>
    public sealed class BodySway : MonoBehaviour
    {
        public MotorDriver Driver;
        /// <summary>Everything that sits on the suspension; the collider and physics stay on the root.</summary>
        public Transform Chassis;
        public float PitchPerAcceleration = .55f, RollPerCornering = .7f, MaxDegrees = 4.5f;
        private const float Settle = .2f, ShakeHertz = 11, ShakeDegrees = .13f, RumbleDegrees = .45f;

        private float lastSpeed, lastYaw, pitch, roll, pitchRate, rollRate;

        private void OnEnable() => lastYaw = transform.eulerAngles.y;

        private void LateUpdate()
        {
            float dt = Time.deltaTime;
            if (dt <= 0) return;
            var view = Driver.View;
            float speed = view.Speed;
            float acceleration = (speed - lastSpeed) / dt;
            float turning = Mathf.DeltaAngle(lastYaw, transform.eulerAngles.y) / dt * Mathf.Deg2Rad;
            lastSpeed = speed; lastYaw = transform.eulerAngles.y;
            // Positive X rotation dips the nose; positive Z rotation drops the left side (the outside of a right turn).
            float wantPitch = Mathf.Clamp(-acceleration * PitchPerAcceleration, -MaxDegrees, MaxDegrees);
            float wantRoll = Mathf.Clamp(speed * turning * RollPerCornering, -MaxDegrees, MaxDegrees);
            pitch = Mathf.SmoothDamp(pitch, wantPitch, ref pitchRate, Settle);
            roll = Mathf.SmoothDamp(roll, wantRoll, ref rollRate, Settle);
            float shake = view.Occupied ? Mathf.Sin(Time.time * ShakeHertz * 2 * Mathf.PI) * ShakeDegrees : 0;
            float pace = Mathf.Clamp01(Mathf.Abs(speed) / 8);
            float rumble = (Mathf.PerlinNoise(Time.time * 7, transform.position.x) - .5f) * RumbleDegrees * pace;
            Chassis.localRotation = Quaternion.Euler(pitch + rumble, 0, roll + shake);
        }
    }
}
