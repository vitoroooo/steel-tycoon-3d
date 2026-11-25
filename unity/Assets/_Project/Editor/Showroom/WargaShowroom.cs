using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Steel.Presentation.Characters.Npc;

namespace Steel.Editor.Showroom
{
    /// <summary>Review sheet of the cartoon townsfolk where they stand in the city, each caught mid-stance.</summary>
    public static class WargaShowroom
    {
        private static readonly string[] Subjects = { "Ngobrol / bicara", "Bapak duduk", "Jongkok pinggir jalan", "Pekerja billet" };

        private static readonly float[] Views = { -30, 30, -75, 75, 0 };   // the first one no wall stands in
        private const float Distance = 4.2f;

        [MenuItem("Steel/Showroom/Kartun warga")]
        public static void Townsfolk()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            foreach (Bystander person in Object.FindObjectsOfType<Bystander>()) person.Pose(person.StartAt * 9);
            int w = ShowroomStage.Width / 2, h = ShowroomStage.Height / 2;
            var camera = new ShowroomCamera(w * 2, h * 2, true);
            camera.Shoot(Vector3.up, 0, 10, 5, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            for (int i = 0; i < Subjects.Length; i++)
            {
                Transform person = GameObject.Find(Subjects[i]).transform;
                Vector3 face = person.position + Vector3.up * .8f;
                float yaw = Views.First(side => !Physics.Linecast(face, face + Quaternion.Euler(0, person.eulerAngles.y + side, 0)
                    * Vector3.forward * Distance)) + person.eulerAngles.y;
                camera.Shoot(face, yaw, 10, Distance,
                    new RectInt(i % 2 * w, i / 2 * h, w, h));
            }
            camera.Save(ShowroomStage.SheetPath("kartun-warga"));
        }
    }
}
