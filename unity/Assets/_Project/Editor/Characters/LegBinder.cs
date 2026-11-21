using System.Linq;
using UnityEngine;
using Steel.Presentation.Characters;

namespace Steel.Editor.Characters
{
    /// <summary>Finds the Rocketbox leg bones so the avatar can sit on a saddle with its feet on rests.</summary>
    internal static class LegBinder
    {
        internal static void Bind(GameObject visual)
        {
            Transform[] bones = visual.GetComponentsInChildren<Transform>();
            var pose = visual.AddComponent<SeatedPose>();
            pose.Hips = visual.transform.Find(RocketboxImportRules.RootBone);
            pose.Head = bones.First(b => b.name == "Bip01 Head");
            pose.LeftLeg = Chain(visual.transform, bones, "L");
            pose.RightLeg = Chain(visual.transform, bones, "R");
        }

        private static LimbChain Chain(Transform root, Transform[] bones, string side)
        {
            Transform Bone(string part) => bones.First(b => b.name == "Bip01 " + side + " " + part);
            Transform foot = Bone("Foot");
            return new LimbChain
            {
                Upper = Bone("Thigh"), Lower = Bone("Calf"), Tip = foot,
                Fingers = new Transform[0], OpenFingers = new Quaternion[0],
                // A rest that faces the way the avatar faces keeps the sole flat, as in the bind pose.
                GripOffset = Quaternion.Inverse(root.rotation) * foot.rotation
            };
        }
    }
}
