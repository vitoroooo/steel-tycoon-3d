using System.Linq;
using UnityEngine;
using Steel.Editor.Models;
using Steel.Presentation.Characters;
using Steel.Presentation.Characters.Toon;

namespace Steel.Editor.Characters.Toon
{
    /// <summary>Builds cartoon people from the authored FBX bodies: rigid parts on joints, posed by code.</summary>
    internal static class ToonCharacter
    {
        internal const string Model = "Characters/Pengepul";

        /// <summary>The player's body: walks, leans, sits on a saddle and holds a bar.</summary>
        internal static GameObject Create(float heightMetres)
        {
            GameObject visual = Walker(Model, heightMetres, out ToonJoints joints);
            ToonLimbs.Bind(visual, joints);
            return visual;
        }

        /// <summary>A body that walks from its own ground speed.</summary>
        internal static GameObject Walker(string model, float heightMetres, out ToonJoints joints)
        {
            GameObject visual = Body(model, heightMetres, out joints);
            visual.AddComponent<LocomotionAnimator>();
            visual.AddComponent<ToonGait>().Joints = joints;
            return visual;
        }

        /// <summary>Just the scaled body standing on y = 0, with its joints found and its rest pose recorded.</summary>
        internal static GameObject Body(string model, float heightMetres, out ToonJoints joints)
        {
            GameObject visual = AuthoredModel.Create(model, _ => ToonCast.Material);
            ToonTree.Build(visual.transform);
            visual.transform.localScale *= heightMetres / Bounds(visual).size.y;
            visual.transform.position = Vector3.down * Bounds(visual).min.y;
            Transform[] parts = visual.GetComponentsInChildren<Transform>();
            Transform Bone(string name) => parts.First(part => part.name == "Bip01" + name);
            Transform[] Pair(string name) => new[] { Bone(" L " + name), Bone(" R " + name) };
            joints = new ToonJoints
            {
                Hips = Bone(""), Spine = Bone(" Spine"), Head = Bone(" Head"),
                Thighs = Pair("Thigh"), Calves = Pair("Calf"), Feet = Pair("Foot"),
                UpperArms = Pair("UpperArm"), Forearms = Pair("Forearm"), Hands = Pair("Hand")
            };
            joints.Capture(visual.transform);
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
