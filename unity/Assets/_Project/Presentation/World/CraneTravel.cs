using UnityEngine;

namespace Steel.Presentation.World
{
    /// <summary>Runs a gantry trolley back and forth with eased starts and stops; the hanging load swings behind it.</summary>
    public sealed class CraneTravel : MonoBehaviour
    {
        public Transform Trolley, Load;
        public float Reach = 5.5f, Speed = 1.1f, Pause = 4;
        private const float Acceleration = .5f, RopeLength = 4.5f, Damping = .6f;

        private float target, speed, wait, angle, swing;
        private Quaternion rest;

        private void Start() { target = Reach; rest = Load.localRotation; }

        private void Update()
        {
            float dt = Time.deltaTime, x = Trolley.localPosition.x, before = speed;
            float remaining = target - x;
            if (wait > 0) { wait -= dt; speed = Mathf.MoveTowards(speed, 0, Acceleration * dt); }
            else
            {
                // Slow down early enough to stop on the mark: v² = 2·a·d.
                float allowed = Mathf.Min(Speed, Mathf.Sqrt(2 * Acceleration * Mathf.Abs(remaining)));
                speed = Mathf.MoveTowards(speed, Mathf.Sign(remaining) * allowed, Acceleration * dt);
                if (Mathf.Abs(remaining) < .05f) { target = -target; wait = Pause; }
            }
            Trolley.localPosition += Vector3.right * speed * dt;
            // A pendulum driven by the trolley's acceleration.
            float push = dt > 0 ? (speed - before) / dt : 0;
            swing += (-Physics.gravity.magnitude / RopeLength * Mathf.Sin(angle) - push / RopeLength * Mathf.Cos(angle)
                - Damping * swing) * dt;
            angle += swing * dt;
            // About the crane's own axis, whichever way the exporter turned the trolley.
            Load.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward) * Trolley.rotation * rest;
        }
    }
}
