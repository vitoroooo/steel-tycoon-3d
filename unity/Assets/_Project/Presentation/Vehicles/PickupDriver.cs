using System;
using UnityEngine;
using Steel.Services;
using Steel.Presentation.Input;
using Steel.Presentation.World;

namespace Steel.Presentation.Vehicles
{
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public sealed class PickupDriver : MonoBehaviour
    {
        private GameHost host;
        private SliceInput input;
        public IMoveInput MoveInput { get; set; }
        private Rigidbody body;
        private BoxCollider box;
        private float speed;
        private Vector3 HalfExtents => Vector3.Scale(box.size * .5f, transform.lossyScale);
        private Vector3 Center(Quaternion rotation) =>
            body.position + rotation * Vector3.Scale(box.center, transform.lossyScale);
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
        private void ResetMotion()
        {
            speed = 0; Vector3 p = body.position;
            if (Physics.Raycast(p + Vector3.up * 3, Vector3.down, out var ground, 8, Physics.DefaultRaycastLayers))
                p.y = ground.point.y + .035f;
            body.position = p; host.Game.ReportVehicle(p.x, p.z, 0);
        }
        private void FixedUpdate()
        {
            var v = host.Game.Vehicle;
            if (!v.Occupied) return;
            if (input.IsBlocked)
            {
                speed = 0; host.Game.ReportVehicle(body.position.x, body.position.z, 0); return;
            }
            Vector2 axes = Vector2.ClampMagnitude(MoveInput.Move, 1);
            float target = axes.y * (axes.y >= 0 ? v.MaxSpeed : v.ReverseSpeed);
            speed = Mathf.MoveTowards(speed, target,
                (Mathf.Abs(axes.y) < .01f ? v.Braking : v.Acceleration) * Time.fixedDeltaTime);
            float turn = axes.x * v.TurnDegrees * Mathf.Clamp(speed / 4, -1, 1) * Time.fixedDeltaTime;
            Quaternion rotation = body.rotation * Quaternion.Euler(0, turn, 0);
            if (RotationBlocked(rotation)) rotation = body.rotation;
            Vector3 delta = rotation * Vector3.forward * speed * Time.fixedDeltaTime;
            if (delta.sqrMagnitude > 0 && Physics.BoxCast(Center(rotation),
                HalfExtents, delta.normalized, out _, rotation,
                delta.magnitude + .08f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            { delta = Vector3.zero; speed = 0; }
            Vector3 position = body.position + delta;
            if (Physics.Raycast(position + Vector3.up * 3, Vector3.down, out var ground, 8,
                Physics.DefaultRaycastLayers)) position.y = ground.point.y + .035f;
            body.MovePosition(position); body.MoveRotation(rotation);
            host.Game.ReportVehicle(position.x, position.z, speed);
        }
        private bool RotationBlocked(Quaternion rotation)
        {
            foreach (var hit in Physics.OverlapBox(Center(rotation), HalfExtents, rotation,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                if (hit.attachedRigidbody != body && !hit.transform.IsChildOf(transform)) return true;
            return false;
        }
        public void Toggle()
        {
            if (!host.Game.Vehicle.Occupied) { host.Game.EnterVehicle(); return; }
            foreach (var local in new[] { new Vector3(-1.7f, 0, 0), new Vector3(1.7f, 0, 0), new Vector3(0, 0, -3) })
            {
                Vector3 p = transform.TransformPoint(local);
                if (Physics.CheckCapsule(p + Vector3.up * .45f, p + Vector3.up * 1.6f, .3f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) continue;
                host.Game.ExitVehicle(p.x, p.z); return;
            }
            throw new InvalidOperationException("Pintu terhalang. Parkir di tempat lebih lapang.");
        }
    }
}
