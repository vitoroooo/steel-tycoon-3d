using System.Linq;
using UnityEngine;

namespace Steel.Editor.Characters.Toon
{
    /// <summary>Rebuilds the limb tree of Pengepul.fbx, which exports its parts side by side at their joints.</summary>
    internal static class ToonTree
    {
        // child → parent, as suffixes of "Bip01"; sides are expanded for L and R.
        private static readonly (string child, string parent)[] Core =
            { (" Spine", ""), (" Head", " Spine") };
        private static readonly (string child, string parent)[] Limbs =
        {
            ("UpperArm", " Spine"), ("Forearm", "UpperArm"), ("Hand", "Forearm"),
            ("Thigh", ""), ("Calf", "Thigh"), ("Foot", "Calf")
        };

        internal static void Build(Transform model)
        {
            Transform[] parts = model.GetComponentsInChildren<Transform>();
            Transform Bone(string suffix) => parts.First(part => part.name == "Bip01" + suffix);
            foreach (var (child, parent) in Core) Bone(child).SetParent(Bone(parent), true);
            foreach (string side in new[] { " L ", " R " })
                foreach (var (child, parent) in Limbs)
                    Bone(side + child).SetParent(Bone(parent.StartsWith(" ") || parent == "" ? parent : side + parent), true);
        }
    }
}
