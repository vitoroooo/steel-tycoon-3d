using UnityEngine;

namespace Steel.Presentation.Characters.Toon
{
    /// <summary>Walks a jointed cartoon body from its ground speed: no clips, no skinning, feet kept on the floor.</summary>
    [DefaultExecutionOrder(-20)]   // first in LateUpdate: lean, seat and arm IK build on this pose
    public sealed class ToonGait : MonoBehaviour
    {
        public ToonJoints Joints;
        [Tooltip("Ground covered by one full cycle (two steps), in metres.")] public float Stride = 1.15f;
        private const float WalkSpeed = 1.4f, RunSpeed = 4.5f, Ease = .12f, Turn = Mathf.PI * 2;
        private LocomotionAnimator locomotion;
        private float phase, effort, effortRate;

        private void OnEnable() => locomotion = GetComponent<LocomotionAnimator>();

        private void LateUpdate()
        {
            Joints.Rest();
            float speed = locomotion.GroundSpeed;
            effort = Mathf.SmoothDamp(effort, Mathf.Clamp01(speed / WalkSpeed), ref effortRate, Ease);
            phase = (phase + speed / Stride * Turn * Time.deltaTime) % Turn;
            ToonIdle.Breathe(Joints, Time.time, 1 - effort);
            if (effort > .001f) Step(Mathf.InverseLerp(WalkSpeed, RunSpeed, speed));
            if (!locomotion.Frozen) Joints.Plant(transform.position.y);
        }

        /// <param name="run">0 at walking pace, 1 at a full run.</param>
        private void Step(float run)
        {
            float lean = Mathf.Lerp(2, 9, run) * effort, twist = Mathf.Sin(phase) * 6 * effort;
            ToonJoints.Bend(Joints.Hips, 0, -twist * .5f, Mathf.Sin(phase) * 3 * effort);
            ToonJoints.Bend(Joints.Spine, lean, twist, 0);
            ToonJoints.Bend(Joints.Head, -lean * .6f, -twist * .6f, 0);
            for (int side = 0; side < 2; side++)
            {
                float cycle = phase + side * Mathf.PI, swing = Mathf.Sin(cycle);
                float reach = swing * Mathf.Lerp(28, 44, run) * effort;
                // The knee folds while the leg travels forward, and straightens before the heel lands.
                float knee = Mathf.Max(0, Mathf.Cos(cycle)) * Mathf.Lerp(38, 70, run) * effort;
                ToonJoints.Bend(Joints.Thighs[side], -reach, 0, 0);
                ToonJoints.Bend(Joints.Calves[side], knee, 0, 0);
                ToonJoints.Bend(Joints.Feet[side], (reach - knee) * .7f, 0, 0);
                // Arms counter the legs; the elbow stays a little bent and folds more at a run.
                ToonJoints.Bend(Joints.UpperArms[side], swing * Mathf.Lerp(24, 42, run) * effort, 0, 0);
                ToonJoints.Bend(Joints.Forearms[side],
                    -(Mathf.Lerp(10, 48, run) + Mathf.Max(0, -swing) * 14) * effort, 0, 0);
            }
        }
    }
}
