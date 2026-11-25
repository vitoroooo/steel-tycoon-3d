using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Steel.Services.Map;

namespace Steel.Editor.Showroom
{
    /// <summary>Six close sheets of Sukabaja from straight above, to spot a bare block. Written to Logs, not kept in the repository.</summary>
    public static class QuarterShowroom
    {
        private const float Height = 325;   // shows a strip 174 m deep
        private const int Columns = 2, Rows = 3;

        [MenuItem("Steel/Showroom/Peta per petak")]
        public static void Survey()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            MapRect land = MapPlan.Load().bounds;
            int w = ShowroomStage.Width, h = ShowroomStage.Height;
            RenderSettings.fog = false;
            for (int column = 0; column < Columns; column++)
                for (int row = 0; row < Rows; row++)
                {
                    var camera = new ShowroomCamera(w, h, true);
                    var middle = new Vector3(Mathf.Lerp(land.west, land.east, (column + .5f) / Columns), 0,
                        Mathf.Lerp(land.south, land.north, (row + .5f) / Rows));
                    camera.Shoot(middle, 180, 89, Height, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
                    camera.Shoot(middle, 180, 89, Height, new RectInt(0, 0, w, h));
                    camera.Save("Logs/petak-" + column + row + ".png");
                }
            RenderSettings.fog = true;
        }
    }
}
