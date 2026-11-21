using UnityEngine;
using Steel.Editor.Characters;
using Steel.Presentation.Characters.Npc;

namespace Steel.Editor.Npc
{
    /// <summary>Puts a bystander in the city: either walking a route or holding a looping pose on the spot.</summary>
    internal static class NpcBuilder
    {
        private const int IgnoreRaycastLayer = 2;   // the orbit camera must not treat people as walls
        private const float BodyRadius = .28f;

        internal static GameObject Walker(string name, CharacterSpec spec, RuntimeAnimatorController gait, float pace,
            params Vector3[] route)
        {
            Vector3 towards = route[1] - route[0];
            var npc = Stand(name, spec, CharacterRig.Walker(spec, gait), route[0], Mathf.Atan2(towards.x, towards.z) * Mathf.Rad2Deg);
            npc.AddComponent<Rigidbody>().isKinematic = true;   // a collider that moves needs one
            var stroller = npc.AddComponent<Stroller>();
            stroller.Route = route; stroller.Pace = pace;
            return npc;
        }

        /// <summary>startAt (0..1) is where in the clip this person begins, so a group is never in step.</summary>
        internal static GameObject Idler(string name, CharacterSpec spec, string clip, Vector3 at, float yaw, float startAt)
        {
            GameObject body = CharacterRig.Body(spec, CharacterAnimator.Pose(clip));
            var bystander = body.AddComponent<Bystander>();
            bystander.StartAt = startAt; bystander.Tempo = .92f + startAt * .16f;
            return Stand(name, spec, body, at, yaw);
        }

        private static GameObject Stand(string name, CharacterSpec spec, GameObject body, Vector3 at, float yaw)
        {
            var npc = new GameObject(name);
            npc.transform.SetParent(CityGeometry.Root, false);
            npc.transform.SetPositionAndRotation(at, Quaternion.Euler(0, yaw, 0));
            body.transform.SetParent(npc.transform, false);
            var capsule = npc.AddComponent<CapsuleCollider>();
            capsule.height = spec.HeightMetres; capsule.radius = BodyRadius; capsule.center = Vector3.up * spec.HeightMetres / 2;
            npc.AddComponent<DistanceHide>().Body = body;
            foreach (Transform part in npc.GetComponentsInChildren<Transform>()) part.gameObject.layer = IgnoreRaycastLayer;
            return npc;
        }
    }
}
