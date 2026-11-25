using UnityEngine;

namespace Steel.Editor
{
    internal static class OverheadWires
    {
        internal static void Create()
        {
            var material = CityMaterials.Plain("Kabel udara", new Color(.025f, .03f, .035f), .3f, .35f);
            foreach (int x in new[] { -7, 7 })
                foreach (int z in new[] { -15, 12 })
                    for (int strand = 0; strand < 2; strand++)
                    {
                        float length = z == -15 ? 27 : 21;
                        var go = new GameObject("Kabel listrik / catenary"); go.transform.SetParent(CityGeometry.Root);
                        var line = go.AddComponent<LineRenderer>(); line.sharedMaterial = material;
                        line.startWidth = line.endWidth = .025f; line.positionCount = 25;
                        for (int i = 0; i < 25; i++)
                        {
                            float t = i / 24f;
                            line.SetPosition(i, new Vector3(x + strand * .18f, 5.9f - 2.5f * t * (1 - t), z + t * length));
                        }
                        go.isStatic = true;
                    }
        }
    }
}
