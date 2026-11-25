using UnityEngine;
using Steel.Presentation.Characters;
using Steel.Presentation.Characters.Toon;

namespace Steel.Editor.Characters.Toon
{
    /// <summary>Gives the cartoon body the arm and leg chains that carts and saddles already drive.</summary>
    internal static class ToonLimbs
    {
        internal static void Bind(GameObject visual, ToonJoints joints)
        {
            var reach = visual.AddComponent<ArmReach>();
            reach.Left = Arm(joints, 0, -1); reach.Right = Arm(joints, 1, 1);
            var pose = visual.AddComponent<SeatedPose>();
            pose.Hips = joints.Hips; pose.Head = joints.Head;
            pose.LeftLeg = Leg(joints, 0); pose.RightLeg = Leg(joints, 1);
            visual.AddComponent<BodyLean>().Spine = joints.Spine;
        }

        /// <summary>The mitten hangs below the wrist with its palm towards the body.</summary>
        private static LimbChain Arm(ToonJoints joints, int side, float sign) => new LimbChain
        {
            Upper = joints.UpperArms[side], Lower = joints.Forearms[side], Tip = joints.Hands[side],
            Fingers = new Transform[0], OpenFingers = new Quaternion[0], CurlAxis = Vector3.forward,
            GripOffset = Quaternion.Inverse(Quaternion.LookRotation(Vector3.down, Vector3.right * sign))
        };

        private static LimbChain Leg(ToonJoints joints, int side) => new LimbChain
        {
            Upper = joints.Thighs[side], Lower = joints.Calves[side], Tip = joints.Feet[side],
            Fingers = new Transform[0], OpenFingers = new Quaternion[0], GripOffset = Quaternion.identity
        };
    }
}
