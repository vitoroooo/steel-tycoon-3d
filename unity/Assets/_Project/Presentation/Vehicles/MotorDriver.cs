using System;
using UnityEngine;
using Steel.Services;
using Steel.Presentation.Input;
using Steel.Presentation.World;

namespace Steel.Presentation.Vehicles
{
    /// <summary>Drives one vehicle of the fleet: throttle, steering, and blocking against the city.</summary>
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public sealed class MotorDriver : MonoBehaviour
    {
        public string VehicleId;
        /// <summary>Local spots tried in order when the driver gets off.</summary>
        public Vector3[] ExitPoints;
        public IMoveInput MoveInput { get; set; }
        /// <summary>Steering input in -1..1; zero while nobody drives.</summary>
        public float Steer { get; private set; }
        public VehicleView View => host.Game.VehicleOf(VehicleId);
        private const float RideHeight = .035f;
        private GameHost host;
        private SliceInput input;
        private Rigidbody body;
        private BoxCollider box;
        private float speed;

        private void Start()
        {
            host = FindObjectOfType<GameHost>();
            input = FindObjectOfType<SliceInput>();
            if (MoveInput == null) MoveInput = input;
            body = GetComponent<Rigidbody>(); body.isKinematic = true;
            box = GetComponent<BoxCollider>();
            body.interpolation = RigidbodyInterpolation.Interpolate;
            GetComponent<FallRecovery>().Recovered += ResetMotion; ResetMotion();
        }

        private void Report(Vector3 p, float metresPerSecond) =>
            host.Game.ReportVehicle(VehicleId, p.x, p.z, metresPerSecond);

        private void ResetMotion()
        {
            speed = 0;
            body.position = MotorPhysics.OnGround(body.position, RideHeight);
            Report(body.position, 0);
        }

        private void FixedUpdate()
        {
            var v = View; Steer = 0;
            if (!v.Occupied) return;
            if (input.IsBlocked) { speed = 0; Report(body.position, 0); return; }
            Vector2 axes = Vector2.ClampMagnitude(MoveInput.Move, 1); Steer = axes.x;
            float target = axes.y * (axes.y >= 0 ? v.MaxSpeed : v.ReverseSpeed);
            speed = Mathf.MoveTowards(speed, target,
                (Mathf.Abs(axes.y) < .01f ? v.Braking : v.Acceleration) * Time.fixedDeltaTime);
            float turn = axes.x * v.TurnDegrees * Mathf.Clamp(speed / 4, -1, 1) * Time.fixedDeltaTime;
            Quaternion rotation = body.rotation * Quaternion.Euler(0, turn, 0);
            if (MotorPhysics.Overlaps(body, box, rotation)) rotation = body.rotation;
            Vector3 delta = rotation * Vector3.forward * speed * Time.fixedDeltaTime;
            if (MotorPhysics.Blocked(body, box, rotation, delta)) { delta = Vector3.zero; speed = 0; }
            Vector3 position = MotorPhysics.OnGround(body.position + delta, RideHeight);
            body.MovePosition(position); body.MoveRotation(rotation);
            Report(position, speed);
        }

        /// <summary>The one context action: buy it, get on, or get off where there is room.</summary>
        public void Toggle()
        {
            var v = View;
            if (!v.Owned) { host.Game.BuyVehicle(VehicleId); return; }
            if (!v.Occupied) { host.Game.EnterVehicle(VehicleId); return; }
            foreach (Vector3 local in ExitPoints)
            {
                Vector3 p = transform.TransformPoint(local);
                if (Physics.CheckCapsule(p + Vector3.up * .45f, p + Vector3.up * 1.6f, .3f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) continue;
                host.Game.ExitVehicle(p.x, p.z); return;
            }
            throw new InvalidOperationException("Tidak ada ruang untuk turun. Parkir di tempat lebih lapang.");
        }
    }
}
