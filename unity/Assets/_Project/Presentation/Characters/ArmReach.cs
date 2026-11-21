using UnityEngine;

namespace Steel.Presentation.Characters
{
    /// <summary>Puts both hands on a pair of grips (push bar, steering bar) on top of the playing clip.</summary>
    [DefaultExecutionOrder(50)]   // after the animator, HipsAnchor and whatever moves the grips
    public sealed class ArmReach : MonoBehaviour
    {
        public ArmChain Left, Right;
        private const float BlendSeconds = .25f;
        private Transform leftGrip, rightGrip;
        private bool holding;
        private float weight;

        public void Hold(Transform left, Transform right)
        {
            leftGrip = left; rightGrip = right; holding = true;
        }

        public void Release() => holding = false;

        /// <summary>Solves at full weight right now; review sheets have no frames to blend over.</summary>
        public void Snap()
        {
            weight = 1;
            Solve();
        }

        private void LateUpdate()
        {
            weight = Mathf.MoveTowards(weight, holding ? 1 : 0, Time.deltaTime / BlendSeconds);
            if (weight > 0) Solve();
        }

        private void Solve()
        {
            Vector3 back = -transform.up - transform.forward * .5f;
            Left.Reach(leftGrip, back - transform.right * .45f, weight);
            Right.Reach(rightGrip, back + transform.right * .45f, weight);
        }
    }
}
