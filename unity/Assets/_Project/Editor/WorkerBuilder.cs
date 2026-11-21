using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Steel.Services;
using Steel.Presentation.Player;

namespace Steel.Editor
{
    internal static class WorkerBuilder
    {
        private const string Path = "Assets/ThirdParty/Quaternius/Worker_Male.fbx";
        private static RuntimeAnimatorController Controller()
        {
            const string path = "Assets/_Project/Generated/Worker.controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            var clips = AssetDatabase.LoadAllAssetsAtPath(Path).OfType<AnimationClip>()
                .Where(c => !c.name.StartsWith("__preview__")).ToArray();
            Debug.Log("WORKER_CLIPS: " + string.Join(", ", clips.Select(c => c.name)));
            AnimationClip Find(string word) => clips.First(c => c.name.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0);
            if (controller != null)
            {
                foreach (var state in controller.layers[0].stateMachine.states) state.state.motion = Find(state.state.name);
                return controller;
            }
            controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            var machine = controller.layers[0].stateMachine;
            var idle = machine.AddState("Idle"); idle.motion = Find("Idle"); machine.defaultState = idle;
            var walk = machine.AddState("Walk"); walk.motion = Find("Walk");
            var move = idle.AddTransition(walk); move.hasExitTime = false; move.duration = .15f;
            move.AddCondition(AnimatorConditionMode.Greater, .2f, "Speed");
            var stop = walk.AddTransition(idle); stop.hasExitTime = false; stop.duration = .15f;
            stop.AddCondition(AnimatorConditionMode.Less, .15f, "Speed"); return controller;
        }
        internal static Transform Create(CityLayout layout)
        {
            var importer = (ModelImporter)AssetImporter.GetAtPath(Path);
            importer.animationType = ModelImporterAnimationType.Generic; importer.importAnimation = true;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.importNormals = ModelImporterNormals.Calculate; importer.normalSmoothingAngle = 100;
            var clips = importer.defaultClipAnimations;
            foreach (var clip in clips)
            { clip.loopTime = true; clip.lockRootRotation = true; clip.lockRootHeightY = true; clip.lockRootPositionXZ = true; }
            importer.clipAnimations = clips; importer.SaveAndReimport();
            var root = new GameObject("Pekerja"); root.layer = 2;
            root.AddComponent<Steel.Presentation.World.FallRecovery>();
            root.transform.position = new Vector3(layout.Spawn.X, layout.SpawnY, layout.Spawn.Z);
            var controller = root.AddComponent<CharacterController>(); controller.height = 1.85f;
            controller.center = Vector3.up * .925f; controller.radius = .3f; controller.stepOffset = .3f;
            var visual = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            visual.name = "Pekerja / Quaternius"; visual.transform.SetParent(root.transform, false);
            var b = WorkerBounds.Get(visual);
            Debug.Log("WORKER_BOUNDS: bind=" + b + " animated=" + ModelLibrary.BoundsOf(visual));
            visual.transform.localScale *= 1.8f / b.size.y;
            b = WorkerBounds.Get(visual); visual.transform.position += root.transform.position -
                new Vector3(b.center.x, b.min.y, b.center.z);
            foreach (var renderer in visual.GetComponentsInChildren<Renderer>())
                renderer.sharedMaterials = renderer.sharedMaterials.Select(WorkerMaterial).ToArray();
            var animator = visual.GetComponent<Animator>() ?? visual.AddComponent<Animator>();
            animator.avatar = AssetDatabase.LoadAllAssetsAtPath(Path).OfType<Avatar>().FirstOrDefault();
            animator.runtimeAnimatorController = Controller();
            animator.applyRootMotion = false; visual.AddComponent<WorkerAnimation>();
            foreach (Transform t in visual.GetComponentsInChildren<Transform>()) t.gameObject.layer = 2;
            root.AddComponent<PlayerMover>().Visual = visual.transform; return root.transform;
        }
        private static Material WorkerMaterial(Material source)
        {
            Color color = source.HasProperty("_Color") ? source.GetColor("_Color") : Color.gray;
            var material = CityMaterials.Plain("Worker " + source.name, color, 0, .28f);
            if (source.HasProperty("_MainTex")) material.mainTexture = source.GetTexture("_MainTex");
            return material;
        }
    }
}
