using UnityEngine;
using Steel.Services;
using Steel.Presentation.Player;
using Steel.Presentation.World;

namespace Steel.Editor.Characters
{
    /// <summary>The controllable pengepul: collider, movement and the avatar from the catalog.</summary>
    internal static class PlayerBuilder
    {
        private const int IgnoreRaycastLayer = 2;
        private const float ColliderRadius = .3f;
        private const float StepHeight = .3f;

        internal static Transform Create(CityLayout layout)
        {
            CharacterSpec spec = CharacterCatalog.Player;
            var root = new GameObject("Pemain");
            root.AddComponent<FallRecovery>();
            root.transform.position = new Vector3(layout.Spawn.X, layout.SpawnY, layout.Spawn.Z);
            var controller = root.AddComponent<CharacterController>();
            controller.height = spec.HeightMetres;
            controller.center = Vector3.up * spec.HeightMetres / 2;
            controller.radius = ColliderRadius;
            controller.stepOffset = StepHeight;
            GameObject visual = CharacterRig.Create(spec, CharacterAnimator.Controller());
            visual.transform.SetParent(root.transform, false);
            foreach (Transform part in root.GetComponentsInChildren<Transform>())
                part.gameObject.layer = IgnoreRaycastLayer;
            root.AddComponent<PlayerMover>().Visual = visual.transform;
            return root.transform;
        }
    }
}
