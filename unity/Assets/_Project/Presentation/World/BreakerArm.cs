using UnityEngine;

namespace Steel.Presentation.World
{
    /// <summary>Works an excavator's breaker: hammers at the concrete, lifts clear, pauses, comes back down and starts again.</summary>
    public sealed class BreakerArm : MonoBehaviour
    {
        public float LiftDegrees = 7, RecoilDegrees = .45f, BlowsPerSecond = 9;
        private const float Hammering = 5, Lifting = 2.4f, Holding = 1.1f, Lowering = 2;
        private const float Cycle = Hammering + Lifting + Holding + Lowering;

        private Quaternion rest;
        private float clock;

        /// <summary>How far the arm is raised above its working position, in degrees.</summary>
        public float Raised { get; private set; }

        private void Start() => rest = transform.localRotation;

        private void Update()
        {
            clock = Mathf.Repeat(clock + Time.deltaTime, Cycle);
            float t = clock;
            if (t < Hammering)
            {
                // Each blow kicks the arm up a little; the kick fades in and out so the phase never starts with a jolt.
                float strength = Mathf.Sin(t / Hammering * Mathf.PI);
                Raised = Mathf.Abs(Mathf.Sin(t * BlowsPerSecond * Mathf.PI)) * RecoilDegrees * strength;
            }
            else if ((t -= Hammering) < Lifting) Raised = Mathf.SmoothStep(0, LiftDegrees, t / Lifting);
            else if ((t -= Lifting) < Holding) Raised = LiftDegrees;
            else Raised = Mathf.SmoothStep(LiftDegrees, 0, (t - Holding) / Lowering);
            // The arm reaches along the machine's +Z, so a negative turn about +X raises it.
            transform.localRotation = Quaternion.AngleAxis(-Raised, Vector3.right) * rest;
        }
    }
}
