using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Steel.Editor.Characters
{
    /// <summary>One shared idle → walk → run blend driven by the "Speed" parameter (m/s).</summary>
    internal static class CharacterAnimator
    {
        private const string ControllerPath = CityMaterials.Generated + "Character.controller";
        /// <summary>Ground speeds the clips were captured at; matching them avoids foot sliding.</summary>
        internal const float WalkSpeed = 1.4f;
        internal const float RunSpeed = 3.1f;

        internal static AnimationClip Clip(string name)
        {
            string path = RocketboxImportRules.Root + "Animations/" + name + ".fbx";
            var clip = AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>()
                .FirstOrDefault(c => !c.name.StartsWith("__preview__"));
            if (clip == null) throw new System.InvalidOperationException("Animation not imported: " + path);
            return clip;
        }

        internal static RuntimeAnimatorController Controller()
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
                controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            }
            // Rebuilt in place so scenes that already reference this asset keep their link.
            var machine = controller.layers[0].stateMachine;
            foreach (var existing in machine.states) machine.RemoveState(existing.state);
            controller.CreateBlendTreeInController("Locomotion", out BlendTree tree);
            tree.blendParameter = "Speed";
            tree.useAutomaticThresholds = false;
            tree.AddChild(Clip("m_idle_neutral_01"), 0);
            tree.AddChild(Clip("m_walk_neutral"), WalkSpeed);
            tree.AddChild(Clip("m_run_neutral"), RunSpeed);
            EditorUtility.SetDirty(controller);
            return controller;
        }
    }
}
