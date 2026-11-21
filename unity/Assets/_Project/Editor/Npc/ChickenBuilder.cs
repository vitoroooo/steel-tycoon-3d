using System.Linq;
using UnityEngine;
using Steel.Editor.Models;
using Steel.Presentation.Characters.Animals;
using Steel.Presentation.Characters.Npc;

namespace Steel.Editor.Npc
{
    internal static class ChickenBuilder
    {
        internal static void Place(Transform player, bool hen, params Vector3[] stops)
        {
            var root = new GameObject(hen ? "Babon kampung" : "Jago kampung").transform;
            root.SetParent(CityGeometry.Root, false);
            var model = AuthoredModel.Create("Animals/Ayam", s => ChickenPaint.Paint(s, hen)).transform;
            model.SetParent(root, false);
            var pose = model.gameObject.AddComponent<ChickenPose>();
            pose.Body = Joint(model, model, "Body");
            pose.Neck = Joint(model, pose.Body, "Leher");
            pose.Tail = Joint(model, pose.Body, "Ekor");
            pose.Left = Joint(model, model, "KakiKiri");
            pose.Right = Joint(model, model, "KakiKanan");
            if (hen)
            {
                model.localScale = Vector3.one * .82f;
                pose.Tail.localScale = new Vector3(.82f, .55f, .5f);
            }
            root.SetPositionAndRotation(stops[0], Quaternion.LookRotation(stops[1] - stops[0]));
            var hide = root.gameObject.AddComponent<DistanceHide>(); hide.Body = model.gameObject; hide.Range = 45;
            var bird = root.gameObject.AddComponent<YardChicken>();
            bird.Player = player; bird.Pose = pose; bird.Stops = stops;
        }

        // FBX mesh axes can differ from Unity's; animate an aligned hinge at the authored mesh origin.
        private static Transform Joint(Transform model, Transform parent, string part)
        {
            var mesh = model.GetComponentsInChildren<MeshRenderer>().Single(m => m.name == part).transform;
            var joint = new GameObject("Sendi " + part).transform;
            joint.SetParent(parent, false); joint.position = mesh.position;
            mesh.SetParent(joint, true); return joint;
        }
    }
}
