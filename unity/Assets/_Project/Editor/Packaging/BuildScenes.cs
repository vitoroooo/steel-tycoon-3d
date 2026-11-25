using System.Linq;
using UnityEditor;

namespace Steel.Editor.Packaging
{
    /// <summary>The scenes of every player, in opening order: the start screens first, then the city.</summary>
    public static class BuildScenes
    {
        public const string Boot = "Assets/_Project/Scenes/Boot.unity", City = "Assets/_Project/Scenes/City.unity";
        public static readonly string[] All = { Boot, City };

        public static void Register() =>
            EditorBuildSettings.scenes = All.Select(path => new EditorBuildSettingsScene(path, true)).ToArray();
    }
}
