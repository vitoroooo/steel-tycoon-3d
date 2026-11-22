using System;
using System.Linq;
using UnityEngine;

namespace Steel.Presentation.Characters.Toon
{
    /// <summary>The joints of a rigid-part cartoon body and the pose they return to every frame.</summary>
    [Serializable]
    public sealed class ToonJoints
    {
        public Transform Hips, Spine, Head;
        public Transform[] Thighs, Calves, Feet, UpperArms, Forearms, Hands;   // left, then right
        private const float PelvisDrop = .5f;
        [SerializeField] private Transform[] all;
        [SerializeField] private Quaternion[] rest;
        [SerializeField] private Vector3 hipsRest;
        [SerializeField] private float ankleHeight, hipsHeight;

        /// <summary>Records the modelled pose; call once, on flat ground, after the body is scaled.</summary>
        public void Capture(Transform body)
        {
            all = new[] { Hips, Spine, Head }.Concat(Thighs).Concat(Calves).Concat(Feet)
                .Concat(UpperArms).Concat(Forearms).Concat(Hands).ToArray();
            rest = all.Select(joint => joint.localRotation).ToArray();
            hipsRest = Hips.localPosition;
            ankleHeight = Feet[0].position.y - body.position.y;
            hipsHeight = Hips.position.y - body.position.y;
        }

        /// <summary>Back to the modelled pose: nothing is driven by clips, so poses must never accumulate.</summary>
        public void Rest()
        {
            for (int i = 0; i < all.Length; i++) all[i].localRotation = rest[i];
            Hips.localPosition = hipsRest;
        }

        /// <summary>Moves the hips so the lower ankle sits at its standing height above the ground.</summary>
        public void Plant(float groundY)
        {
            float lowest = Mathf.Min(Feet[0].position.y, Feet[1].position.y);
            Hips.position += Vector3.up * (groundY + ankleHeight - lowest);
        }

        /// <summary>Rests the seat of the trousers on a surface; it hangs half a hip-height below the hip joints.</summary>
        public void Sit(float seatY) => Hips.position += Vector3.up * (seatY + hipsHeight * PelvisDrop - Hips.position.y);

        /// <summary>Adds a rotation in the joint's own axes: pitch forward, yaw right, roll.</summary>
        public static void Bend(Transform joint, float pitch, float yaw, float roll) =>
            joint.localRotation *= Quaternion.Euler(pitch, yaw, roll);
    }
}
