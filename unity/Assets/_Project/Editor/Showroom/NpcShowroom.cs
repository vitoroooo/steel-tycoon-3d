using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Steel.Editor.Showroom
{
    /// <summary>Review sheet of the townsfolk where they stand in the city, each frozen part-way through their clip.</summary>
    public static class NpcShowroom
    {
        [MenuItem("Steel/Showroom/Warga")]
        public static void Townsfolk()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            foreach (var animator in Object.FindObjectsOfType<Animator>())
            {
                // The editor does not run animators; walkers show their walk, everyone else their only clip.
                var clips = animator.runtimeAnimatorController.animationClips;
                var clip = clips[Mathf.Min(1, clips.Length - 1)];
                clip.SampleAnimation(animator.gameObject, clip.length * .35f);
            }
            int w = ShowroomStage.Width, h = ShowroomStage.Height, tw = w / 3, th = h / 2;
            var camera = new ShowroomCamera(w, h, true);
            Vector3 chat = new Vector3(-10.8f, 1.1f, 35.4f);
            camera.Shoot(chat, 80, 6, 5, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(chat, 80, 6, 5, new RectInt(0, th, tw, th));                                    // two men chatting
            camera.Shoot(new Vector3(-36.3f, .9f, 20), 240, 8, 4.5f, new RectInt(tw, th, tw, th));       // on the bench
            camera.Shoot(new Vector3(10.3f, .8f, 2.5f), 250, 8, 4.5f, new RectInt(tw * 2, th, tw, th));  // squatting by the road
            camera.Shoot(new Vector3(-9, 1, 46), 60, 6, 6, new RectInt(0, 0, tw, th));                   // the shopper walking
            camera.Shoot(new Vector3(29, 1.2f, -27.5f), 200, 10, 9, new RectInt(tw, 0, tw, th));           // smelter hands
            camera.Shoot(new Vector3(-10, 1.6f, 30), 20, 14, 26, new RectInt(tw * 2, 0, tw, th));        // the pavement from afar
            camera.Save(ShowroomStage.SheetPath("warga"));
        }
    }
}
