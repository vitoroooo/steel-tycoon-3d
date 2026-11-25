using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Steel.Editor.Characters;
using Steel.Presentation.Characters;
using Steel.Presentation.Player;
using Steel.Presentation.Vehicles;

namespace Steel.Editor.Showroom
{
    /// <summary>Review sheets shot inside the generated city, to judge assets in their real lighting.</summary>
    public static class WorldShowroom
    {
        [MenuItem("Steel/Showroom/Gerobak di kota")]
        public static void PushingCart()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            var mover = Object.FindObjectOfType<PlayerMover>();
            var cart = Object.FindObjectOfType<CartPusher>();
            Transform visual = mover.Visual;
            visual.rotation = Quaternion.Euler(0, 270, 0);   // open ground west of the spawn point
            foreach (var skin in visual.GetComponentsInChildren<SkinnedMeshRenderer>())
                skin.forceMatrixRecalculationPerRender = true;
            var animator = visual.GetComponent<Animator>();
            animator.Rebind();
            animator.SetFloat("Speed", CharacterAnimator.WalkSpeed);
            animator.Update(0);
            animator.Update(.3f);
            visual.GetComponent<HipsAnchor>().Apply();
            cart.transform.SetPositionAndRotation(
                new Vector3(visual.position.x, cart.transform.position.y, visual.position.z) +
                visual.forward * cart.Reach, visual.rotation * Quaternion.Euler(CartPusher.LiftDegrees, 0, 0));
            var arms = visual.GetComponent<ArmReach>();
            arms.Hold(cart.GripLeft, cart.GripRight);
            arms.Snap();
            int w = ShowroomStage.Width, h = ShowroomStage.Height;
            var camera = new ShowroomCamera(w, h);
            Vector3 focus = visual.position + visual.forward * .8f + Vector3.up * .9f;
            float yaw = visual.eulerAngles.y;
            camera.Shoot(focus, yaw + 140, 12, 7, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(focus, yaw + 140, 12, 7, new RectInt(0, 0, w / 2, h));
            camera.Shoot(focus, yaw + 60, 10, 6, new RectInt(w / 2, h / 2, w / 2, h / 2));
            camera.Shoot(focus + Vector3.up * .2f, yaw + 115, 22, 2.6f, new RectInt(w / 2, 0, w / 2, h / 2));
            camera.Save(ShowroomStage.SheetPath("gerobak-di-kota"));
        }
    }
}
