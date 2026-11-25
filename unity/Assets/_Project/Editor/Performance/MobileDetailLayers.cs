using Steel.Presentation.World.Mobile;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Steel.Editor.Performance
{
    public sealed class MobileDetailLayers : IProcessSceneWithReport
    {
        public int callbackOrder => 10;
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if (report != null && report.summary.platform == BuildTarget.Android) Apply(scene);
        }
        internal static void Apply(Scene scene)
        {
            int trim = LayerMask.NameToLayer(MobileView.TrimLayer), prop = LayerMask.NameToLayer(MobileView.PropLayer);
            int a = 0, b = 0;
            foreach (var root in scene.GetRootGameObjects())
                foreach (var r in root.GetComponentsInChildren<MeshRenderer>(true))
                {
                    if (r.gameObject.layer != 0 || r.GetComponent<Collider>() || !r.GetComponent<MeshFilter>()) continue;
                    var size = r.bounds.size; float span = Mathf.Max(size.x, Mathf.Max(size.y,size.z));
                    if (span <= 1.4f) { r.gameObject.layer = trim; a++; }
                    else if (span <= 3.2f) { r.gameObject.layer = prop; b++; }
                }
            Debug.Log($"MOBILE_DETAIL {a} trim @24m, {b} props @48m");
        }
    }
}
