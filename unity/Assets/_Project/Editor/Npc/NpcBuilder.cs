using UnityEngine;
using Steel.Editor.Characters.Toon;
using Steel.Presentation.Characters.Npc;
using Steel.Presentation.Characters.Toon;

namespace Steel.Editor.Npc
{
    /// <summary>Puts a bystander in the city: either walking a route or holding a looping pose on the spot.</summary>
    internal static class NpcBuilder
    {
        private const int IgnoreRaycastLayer = 2;   // the orbit camera must not treat people as walls
        private const float BodyRadius = .28f;

        internal static GameObject Walker(string name, ToonCitizen who, float pace, params Vector3[] route)
        {
            Vector3 towards = route[1] - route[0];
            GameObject body = ToonCharacter.Walker(who.Model, who.HeightMetres, out _);
            var npc = Stand(name, who, body, route[0], Mathf.Atan2(towards.x, towards.z) * Mathf.Rad2Deg);
            npc.AddComponent<Rigidbody>().isKinematic = true;   // a collider that moves needs one
            var stroller = npc.AddComponent<Stroller>();
            stroller.Route = route; stroller.Pace = pace;
            return npc;
        }

        /// <summary>startAt (0..1) is where in the loop this person begins, so a group is never in step.</summary>
        internal static GameObject Idler(string name, ToonCitizen who, Stance stance, Vector3 at, float yaw, float startAt,
            float seatHeight = 0)
        {
            GameObject body = ToonCharacter.Body(who.Model, who.HeightMetres, out ToonJoints joints);
            var bystander = body.AddComponent<Bystander>();
            bystander.Joints = joints; bystander.Stance = stance; bystander.SeatHeight = seatHeight;
            bystander.StartAt = startAt; bystander.Tempo = .92f + startAt * .16f;
            return Stand(name, who, body, at, yaw);
        }

        private static GameObject Stand(string name, ToonCitizen who, GameObject body, Vector3 at, float yaw)
        {
            var npc = new GameObject(name);
            npc.transform.SetParent(CityGeometry.Root, false);
            npc.transform.SetPositionAndRotation(at, Quaternion.Euler(0, yaw, 0));
            body.transform.SetParent(npc.transform, false);
            var capsule = npc.AddComponent<CapsuleCollider>();
            capsule.height = who.HeightMetres; capsule.radius = BodyRadius; capsule.center = Vector3.up * who.HeightMetres / 2;
            npc.AddComponent<DistanceHide>().Body = body;
            foreach (Transform part in npc.GetComponentsInChildren<Transform>()) part.gameObject.layer = IgnoreRaycastLayer;
            return npc;
        }
    }
}
