using UnityEngine;

namespace Steel.Presentation.Characters
{
    /// <summary>Leans the torso into starts, stops and turns, on top of whatever clip is playing.</summary>
    [DefaultExecutionOrder(20)]   // after the Animator has posed the spine, before the arm and leg IK
    public sealed class BodyLean : MonoBehaviour
    {
        public Transform Spine;
        private const float PitchPerAcceleration = .55f;   // degrees per m/s²
        private const float RollPerTurn = .028f;           // degrees per degree/second of turning, at walking pace
        private const float MaxPitch = 8, MaxRoll = 6, Settle = .16f, WalkingPace = 3;

        private LocomotionAnimator locomotion;
        private Vector3 lastPosition, velocity;
        private float lastYaw, pitch, roll, pitchRate, rollRate;

        private void OnEnable()
        {
            locomotion = GetComponent<LocomotionAnimator>();
            lastPosition = transform.position; lastYaw = transform.eulerAngles.y; velocity = Vector3.zero;
        }

        private void LateUpdate()
        {
            float dt = Time.deltaTime;
            if (dt <= 0) return;
            Vector3 now = (transform.position - lastPosition) / dt; now.y = 0;
            float push = Vector3.Dot(now - velocity, transform.forward) / dt;
            float turning = Mathf.DeltaAngle(lastYaw, transform.eulerAngles.y) / dt;
            velocity = now; lastPosition = transform.position; lastYaw = transform.eulerAngles.y;
            bool carried = locomotion != null && locomotion.Frozen;   // a vehicle moves the avatar; no walking lean
            float wantPitch = carried ? 0 : Mathf.Clamp(push * PitchPerAcceleration, -MaxPitch, MaxPitch);
            float wantRoll = carried ? 0 : Mathf.Clamp(-turning * RollPerTurn * now.magnitude / WalkingPace, -MaxRoll, MaxRoll);
            pitch = Mathf.SmoothDamp(pitch, wantPitch, ref pitchRate, Settle);
            roll = Mathf.SmoothDamp(roll, wantRoll, ref rollRate, Settle);
            Spine.rotation = Quaternion.AngleAxis(pitch, transform.right) * Quaternion.AngleAxis(roll, transform.forward) * Spine.rotation;
        }
    }
}
