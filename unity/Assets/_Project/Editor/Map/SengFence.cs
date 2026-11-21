using UnityEngine;
using Steel.Editor.Buildings;
using Steel.Editor.Models;

namespace Steel.Editor.Map
{
    /// <summary>Runs of patched corrugated-iron fence; one collider per run keeps the physics cheap.</summary>
    internal static class SengFence
    {
        private const float PanelWidth = 2.4f, Height = 2;

        /// <summary>Stands panels edge to edge from one point to the other, good side facing left of travel.</summary>
        internal static void Run(Vector3 from, Vector3 to)
        {
            Vector3 along = to - from;
            int panels = Mathf.Max(1, Mathf.RoundToInt(along.magnitude / PanelWidth));
            Quaternion facing = Quaternion.LookRotation(Vector3.Cross(Vector3.up, along.normalized));
            var run = new GameObject("Pagar seng"); run.transform.SetParent(CityGeometry.Root, false);
            run.transform.SetPositionAndRotation((from + to) / 2, facing);
            for (int i = 0; i < panels; i++)
            {
                var panel = AuthoredModel.Create("Buildings/PagarSeng", YardMaterials.Fence);
                panel.transform.SetParent(run.transform, false);
                panel.transform.localPosition = Vector3.right * ((i + .5f) / panels - .5f) * along.magnitude;
                panel.transform.localScale = new Vector3(along.magnitude / panels / PanelWidth, 1, 1);
                foreach (Transform part in panel.GetComponentsInChildren<Transform>()) part.gameObject.isStatic = true;
            }
            var box = run.AddComponent<BoxCollider>();
            box.center = Vector3.up * Height / 2; box.size = new Vector3(along.magnitude, Height, .2f);
            run.isStatic = true;
        }
    }
}
