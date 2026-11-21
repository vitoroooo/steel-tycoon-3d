using System.Linq;
using UnityEditor;
using UnityEngine;
using Steel.Presentation.Characters;

namespace Steel.Editor.Characters
{
    /// <summary>Instantiates an avatar with game materials, world scale and the shared animator.</summary>
    internal static class CharacterRig
    {
        internal static GameObject Create(CharacterSpec spec, RuntimeAnimatorController controller)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(spec.ModelPath);
            if (prefab == null) throw new System.InvalidOperationException("Model not imported: " + spec.ModelPath);
            var visual = Object.Instantiate(prefab);
            visual.name = spec.Folder;
            foreach (var renderer in visual.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.sharedMaterials = renderer.sharedMaterials
                    .Select(m => CharacterMaterials.For(spec, m.name)).ToArray();
                renderer.updateWhenOffscreen = true;
            }
            Bounds bounds = SkinnedBounds.Get(visual);
            visual.transform.localScale *= spec.HeightMetres / bounds.size.y;
            visual.transform.position = Vector3.down * SkinnedBounds.Get(visual).min.y;
            if (!visual.TryGetComponent(out Animator animator)) animator = visual.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            visual.AddComponent<LocomotionAnimator>();
            visual.AddComponent<HipsAnchor>().Bind(visual.transform.Find(RocketboxImportRules.RootBone));
            return visual;
        }
    }
}
