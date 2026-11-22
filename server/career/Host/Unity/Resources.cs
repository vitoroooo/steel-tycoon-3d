using System.IO;

namespace UnityEngine
{
    /// <summary>A text file of the game's Resources folder.</summary>
    public class TextAsset
    {
        public string text;
    }

    /// <summary>Reads the game's own Resources folder from disk, so server and phone share one set of rules and one map.</summary>
    public static class Resources
    {
        /// <summary>The folder holding Config/ and Map/. Set once before the first career is opened.</summary>
        public static string Root = "";

        public static T Load<T>(string path) where T : TextAsset, new()
        {
            string file = Path.Combine(Root, path + ".json");
            return File.Exists(file) ? new T { text = File.ReadAllText(file) } : null;
        }
    }
}
