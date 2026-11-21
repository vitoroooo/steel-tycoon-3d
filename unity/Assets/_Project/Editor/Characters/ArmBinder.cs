using System.Linq;
using UnityEngine;
using Steel.Presentation.Characters;

namespace Steel.Editor.Characters
{
    /// <summary>Finds the Rocketbox arm bones and measures how each hand must turn to hold a bar.</summary>
    internal static class ArmBinder
    {
        private static readonly string[] FingerJoints =
            { "1", "11", "12", "2", "21", "22", "3", "31", "32", "4", "41", "42" };

        internal static void Bind(GameObject visual)
        {
            var reach = visual.AddComponent<ArmReach>();
            reach.Left = Chain(visual.transform, "L", -1);
            reach.Right = Chain(visual.transform, "R", 1);
        }

        private static ArmChain Chain(Transform root, string side, float sign)
        {
            Transform[] bones = root.GetComponentsInChildren<Transform>();
            Transform Bone(string part) => bones.First(b => b.name == "Bip01 " + side + " " + part);
            Transform hand = Bone("Hand");
            Vector3 fingers = hand.InverseTransformDirection(Bone("Finger2").position - hand.position).normalized;
            Vector3 thumb = hand.InverseTransformDirection(Bone("Finger0").position - hand.position).normalized;
            // The thumb sits on opposite sides of the two hands, so the cross product flips with the side.
            Vector3 palm = sign * Vector3.Cross(fingers, thumb).normalized;
            Transform[] joints = FingerJoints.Select(j => Bone("Finger" + j)).ToArray();
            return new ArmChain
            {
                Upper = Bone("UpperArm"), Fore = Bone("Forearm"), Hand = hand, Fingers = joints,
                OpenFingers = joints.Select(j => j.localRotation).ToArray(),
                GripOffset = Quaternion.Inverse(Quaternion.LookRotation(fingers, -palm)),
                CurlAxis = Vector3.Cross(fingers, palm).normalized
            };
        }
    }
}
