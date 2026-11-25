using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Steel.Editor.Plan.Places.Builders.Landmarks;

namespace Steel.Editor.Showroom
{
    /// <summary>One sheet of the city's landmarks, each seen from the street in front of its name board.</summary>
    public static class LandmarkShowroom
    {
        private static readonly string[] Boards = { Station.Board, Fuel.Board, TollGate.Board };
        private static readonly float[] Sides = { -30, 20 };
        private const float Distance = 40, Pitch = 24;

        [MenuItem("Steel/Showroom/Bangunan khas")]
        public static void Sheet()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            int w = ShowroomStage.Width, h = ShowroomStage.Height, tileW = w / Sides.Length, tileH = h / Boards.Length;
            var camera = new ShowroomCamera(w, h, true);
            RenderSettings.fog = false;
            for (int row = 0; row < Boards.Length; row++)
            {
                Transform board = Object.FindObjectsOfType<TextMesh>().First(t => t.text == Boards[row]).transform;
                Vector3 focus = new Vector3(board.position.x, 2.5f, board.position.z) + board.forward * 4;
                for (int column = 0; column < Sides.Length; column++)
                {
                    float yaw = board.eulerAngles.y + 180 + Sides[column];
                    camera.Shoot(focus, yaw, Pitch, Distance, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
                    camera.Shoot(focus, yaw, Pitch, Distance, new RectInt(column * tileW, (Boards.Length - 1 - row) * tileH, tileW, tileH));
                }
            }
            RenderSettings.fog = true;
            camera.Save(ShowroomStage.SheetPath("bangunan-khas"));
        }
    }
}
