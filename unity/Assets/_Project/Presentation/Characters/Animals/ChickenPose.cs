using UnityEngine;

namespace Steel.Presentation.Characters.Animals
{
    public sealed class ChickenPose : MonoBehaviour
    {
        public Transform Body, Neck, Tail, Left, Right;
        private float phase, step, blend;
        private Vector3 hip;

        private void Awake()
        {
            hip = Body.localPosition;
            phase = transform.position.z * 1.7f;
        }

        public void Apply(float pace, bool feeding)
        {
            float dt = Time.deltaTime;
            phase += dt * 4;
            step += pace * dt * 22;
            blend = Mathf.MoveTowards(blend, feeding ? 1 : 0, dt * 3);
            float dip = blend * Mathf.SmoothStep(0, 1, Mathf.Max(0, Mathf.Sin(phase)));
            float gait = Mathf.Min(1, pace * 3), swing = gait * Mathf.Sin(step);
            Body.localPosition = hip + Vector3.up * (.008f * gait * Mathf.Abs(swing) - .048f * dip);
            Body.localRotation = Quaternion.Euler(53 * dip + 2 * swing, 0, 2 * swing);
            Neck.localRotation = Quaternion.Euler(30 * dip - 6 * swing, 8 * Mathf.Cos(phase * .55f), 0);
            Tail.localRotation = Quaternion.Euler(-12 * dip + 3 * swing, 2 * Mathf.Sin(phase), 0);
            Left.localRotation = Quaternion.Euler(28 * swing, 0, 0);
            Right.localRotation = Quaternion.Euler(-28 * swing, 0, 0);
        }
    }
}
