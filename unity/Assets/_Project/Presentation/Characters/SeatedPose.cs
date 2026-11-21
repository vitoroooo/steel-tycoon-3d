using UnityEngine;

namespace Steel.Presentation.Characters
{
    /// <summary>Sits the avatar on a saddle: hips on the seat, feet on the rests, back leaning in.</summary>
    [DefaultExecutionOrder(40)]   // after the animator and HipsAnchor, before ArmReach places the hands
    public sealed class SeatedPose : MonoBehaviour
    {
        public Transform Hips, Head;
        public LimbChain LeftLeg, RightLeg;
        private const float LeanDegrees = 28, HeadUpDegrees = 18, KneeSpread = .35f;
        private Transform seat, leftRest, rightRest;

        public bool Sitting => seat != null;

        /// <summary>seat is where the hip joints go; the rests are where the ankles go.</summary>
        public void Sit(Transform seat, Transform leftRest, Transform rightRest)
        {
            this.seat = seat; this.leftRest = leftRest; this.rightRest = rightRest;
            GetComponent<LocomotionAnimator>().Frozen = true;
        }

        public void Stand()
        {
            seat = null;
            GetComponent<LocomotionAnimator>().Frozen = false;
        }

        /// <summary>Applies the pose right now; review sheets have no frames to wait for.</summary>
        public void Apply()
        {
            Hips.rotation = Quaternion.AngleAxis(LeanDegrees, transform.right) * Hips.rotation;
            Head.rotation = Quaternion.AngleAxis(-HeadUpDegrees, transform.right) * Head.rotation;   // eyes on the road
            Hips.position += seat.position - (LeftLeg.Upper.position + RightLeg.Upper.position) / 2;
            Vector3 knee = transform.forward + transform.up;
            LeftLeg.Reach(leftRest, knee - transform.right * KneeSpread, 1);
            RightLeg.Reach(rightRest, knee + transform.right * KneeSpread, 1);
        }

        private void LateUpdate()
        {
            if (Sitting) Apply();
        }
    }
}
