using System.Linq;
using UnityEngine;
using Steel.Editor.Models;
using Steel.Presentation.Characters;
using Steel.Presentation.Characters.Toon;

namespace Steel.Editor.Characters.Toon
{
    /// <summary>Builds the cartoon pengepul from Pengepul.fbx: rigid parts on joints, posed by code.</summary>
    internal static class ToonCharacter
    {
        internal const string Model = "Characters/Pengepul";

        /// <summary>The player's body: walks, leans, sits on a saddle and holds a bar.</summary>
        internal static GameObject Create(float heightMetres)
        {
            GameObject visual = AuthoredModel.Create(Model, _ => ToonCast.Material);
            ToonTree.Build(visual.transform);
            Bounds bounds = Bounds(visual);
            visual.transform.localScale *= heightMetres / bounds.size.y;
            visual.transform.position = Vector3.down * Bounds(visual).min.y;
            Transform[] parts = visual.GetComponentsInChildren<Transform>();
            Transform Bone(string name) => parts.First(part => part.name == "Bip01" + name);
            Transform[] Pair(string name) => new[] { Bone(" L " + name), Bone(" R " + name) };
            visual.AddComponent<LocomotionAnimator>();
            var gait = visual.AddComponent<ToonGait>();
            gait.Joints = new ToonJoints
            {
                Hips = Bone(""), Spine = Bone(" Spine"), Head = Bone(" Head"),
                Thighs = Pair("Thigh"), Calves = Pair("Calf"), Feet = Pair("Foot"),
                UpperArms = Pair("UpperArm"), Forearms = Pair("Forearm"), Hands = Pair("Hand")
            };
            gait.Joints.Capture(visual.transform);
            ToonLimbs.Bind(visual, gait.Joints);
            return visual;
        }

        private static Bounds Bounds(GameObject visual)
        {
            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
            Bounds bounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers) bounds.Encapsulate(renderer.bounds);
            return bounds;
        }
    }
}
