using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Steel.Editor.Characters
{
    /// <summary>Animator controllers built from the Rocketbox clips: locomotion blends driven by "Speed" (m/s), and single looping poses.</summary>
    internal static class CharacterAnimator
    {
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

        /// <summary>The men's idle → walk → run blend; the player uses it too.</summary>
        internal static RuntimeAnimatorController Controller() =>
            Locomotion("Character", "m_idle_neutral_01", "m_walk_neutral", "m_run_neutral");

        /// <summary>Women have their own skeleton proportions, so their own captured clips.</summary>
        internal static RuntimeAnimatorController Female() => Locomotion("CharacterFemale", "f_idle_neutral_01", "f_walk_neutral", null);

        /// <summary>A controller that loops one clip: sitting, talking, squatting.</summary>
        internal static RuntimeAnimatorController Pose(string clip)
        {
            var machine = Reset("Pose " + clip, out AnimatorController controller);
            machine.AddState(clip).motion = Clip(clip);
            EditorUtility.SetDirty(controller);
            return controller;
        }

        private static RuntimeAnimatorController Locomotion(string name, string idle, string walk, string run)
        {
            Reset(name, out AnimatorController controller);
            controller.CreateBlendTreeInController("Locomotion", out BlendTree tree);
            tree.blendParameter = "Speed";
            tree.useAutomaticThresholds = false;
            tree.AddChild(Clip(idle), 0);
            tree.AddChild(Clip(walk), WalkSpeed);
            if (run != null) tree.AddChild(Clip(run), RunSpeed);
            EditorUtility.SetDirty(controller);
            return controller;
        }

        /// <summary>Rebuilt in place so scenes that already reference the asset keep their link.</summary>
        private static AnimatorStateMachine Reset(string name, out AnimatorController controller)
        {
            string path = CityMaterials.Generated + name + ".controller";
            controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(path);
                controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            }
            var machine = controller.layers[0].stateMachine;
            foreach (var existing in machine.states) machine.RemoveState(existing.state);
            return machine;
        }
    }
}
