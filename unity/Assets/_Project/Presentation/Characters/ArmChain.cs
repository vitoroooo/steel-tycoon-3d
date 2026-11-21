using System;
using UnityEngine;

namespace Steel.Presentation.Characters
{
    /// <summary>Shoulder, elbow, wrist and fingers of one arm, solved towards a grip after animation.</summary>
    [Serializable]
    public sealed class ArmChain
    {
        public Transform Upper, Fore, Hand;
        /// <summary>Finger joints without the thumb, parents before children.</summary>
        public Transform[] Fingers;
        /// <summary>Hand rotation relative to a grip whose forward is the fingers and up the back of the hand.</summary>
        public Quaternion GripOffset = Quaternion.identity;
        /// <summary>Hand-local axis the fingers close around.</summary>
        public Vector3 CurlAxis;
        public Quaternion[] OpenFingers;
        private const float CurlDegrees = 48;

        public void Reach(Transform grip, Vector3 elbowHint, float weight)
        {
            Vector3 target = Vector3.Lerp(Hand.position, grip.position, weight);
            Bend(target);
            Upper.rotation = Quaternion.FromToRotation(Hand.position - Upper.position, target - Upper.position) *
                Upper.rotation;
            PointElbow(target, elbowHint, weight);
            Hand.rotation = Quaternion.Slerp(Hand.rotation, grip.rotation * GripOffset, weight);
            Quaternion curl = Quaternion.AngleAxis(CurlDegrees * weight, Hand.TransformDirection(CurlAxis));
            for (int i = 0; i < Fingers.Length; i++)
            {
                Fingers[i].localRotation = OpenFingers[i];   // clips may not key fingers; never accumulate
                Fingers[i].rotation = curl * Fingers[i].rotation;
            }
        }

        /// <summary>Opens or closes the elbow until the wrist is as far from the shoulder as the target.</summary>
        private void Bend(Vector3 target)
        {
            Vector3 toShoulder = Upper.position - Fore.position, toWrist = Hand.position - Fore.position;
            float upper = toShoulder.magnitude, fore = toWrist.magnitude;
            float span = Mathf.Clamp(Vector3.Distance(Upper.position, target), .05f, (upper + fore) * .999f);
            float cosine = (upper * upper + fore * fore - span * span) / (2 * upper * fore);
            float wanted = Mathf.Acos(Mathf.Clamp(cosine, -1, 1)) * Mathf.Rad2Deg;
            Vector3 hinge = Vector3.Cross(toShoulder, toWrist);
            if (hinge.sqrMagnitude < 1e-8f) hinge = Fore.up;
            Fore.rotation = Quaternion.AngleAxis(wanted - Vector3.Angle(toShoulder, toWrist), hinge.normalized) *
                Fore.rotation;
        }

        /// <summary>Spins the arm around the shoulder-to-wrist line; the wrist stays where it is.</summary>
        private void PointElbow(Vector3 target, Vector3 hint, float weight)
        {
            Vector3 line = (target - Upper.position).normalized;
            Vector3 elbow = Vector3.ProjectOnPlane(Fore.position - Upper.position, line);
            Vector3 wanted = Vector3.ProjectOnPlane(hint, line);
            float angle = Vector3.SignedAngle(elbow, wanted, line) * weight;
            Upper.rotation = Quaternion.AngleAxis(angle, line) * Upper.rotation;
        }
    }
}
