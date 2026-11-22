#if UNITY_ANDROID
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor.Android;

namespace Steel.Editor
{
    // Managed HttpClient uses Android sockets, including for the loopback USB connection.
    public sealed class NetworkAndroidManifest : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 0;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string manifest = Path.Combine(path, "src", "main", "AndroidManifest.xml");
            var document = XDocument.Load(manifest);
            XNamespace android = "http://schemas.android.com/apk/res/android";
            const string internet = "android.permission.INTERNET";
            if (document.Root.Elements("uses-permission").Any(p => (string)p.Attribute(android + "name") == internet))
                return;
            document.Root.AddFirst(new XElement("uses-permission", new XAttribute(android + "name", internet)));
            document.Save(manifest);
        }
    }
}
#endif
