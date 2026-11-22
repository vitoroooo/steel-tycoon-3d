using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Steel.Editor.Census
{
    /// <summary>Counts what the city costs a phone: meshes and triangles in all, and what one camera sees from set viewpoints.</summary>
    public static class CityCensus
    {
        private const float PhoneFarClip = 150, EyeHeight = 3, FieldOfView = 60, Aspect = 20f / 9;
        private const int Groups = 30;
        // Where the load is heaviest: the centre roundabout, the old town, the kampung, the industry, the player plots.
        private static readonly Vector3[] Viewpoints =
        {
            new Vector3(-20, EyeHeight, 150), new Vector3(0, EyeHeight, 0), new Vector3(-113, EyeHeight, 51),
            new Vector3(170, EyeHeight, 60), new Vector3(-90, EyeHeight, -112)
        };

        [MenuItem("Steel/Census/Beban kota")]
        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            MeshRenderer[] renderers = Object.FindObjectsOfType<MeshRenderer>()
                .Where(r => r.TryGetComponent(out MeshFilter filter) && filter.sharedMesh).ToArray();
            Debug.Log("CENSUS all: " + renderers.Length + " renderers, " + renderers.Sum(Triangles) + " triangles, "
                + Object.FindObjectsOfType<Collider>().Length + " colliders, "
                + Object.FindObjectsOfType<MeshCollider>().Sum(c => c.sharedMesh ? c.sharedMesh.triangles.Length / 3 : 0) + " collider triangles");
            Debug.Log("CENSUS text: " + Object.FindObjectsOfType<TextMesh>().Length + " signs, each its own draw");
            foreach (var group in renderers.GroupBy(Group).OrderByDescending(g => g.Sum(Triangles)).Take(Groups))
                Debug.Log("CENSUS root " + group.Key + ": " + group.Count() + " renderers, " + group.Sum(Triangles) + " triangles");
            var eye = new GameObject("Census eye").AddComponent<Camera>();
            eye.farClipPlane = PhoneFarClip; eye.fieldOfView = FieldOfView; eye.aspect = Aspect;
            foreach (Vector3 spot in Viewpoints)
                for (int turn = 0; turn < 4; turn++)
                {
                    eye.transform.SetPositionAndRotation(spot, Quaternion.Euler(8, turn * 90, 0));
                    Plane[] planes = GeometryUtility.CalculateFrustumPlanes(eye);
                    MeshRenderer[] seen = renderers.Where(r => GeometryUtility.TestPlanesAABB(planes, r.bounds)).ToArray();
                    Debug.Log("CENSUS view " + spot.ToString("F0") + " yaw " + turn * 90 + ": " + seen.Length + " renderers, "
                        + seen.Sum(Triangles) + " triangles, " + seen.Select(r => r.sharedMaterial).Distinct().Count() + " materials");
                }
            Object.DestroyImmediate(eye.gameObject);
        }

        /// <summary>The top-level object and, inside the city, the mesh a renderer draws: what to lighten first.</summary>
        private static string Group(MeshRenderer renderer) =>
            renderer.transform.root.name + " / " + renderer.GetComponent<MeshFilter>().sharedMesh.name + " in " + (renderer.transform.parent ? renderer.transform.parent.name : "-") + " in " + (renderer.transform.parent && renderer.transform.parent.parent ? renderer.transform.parent.parent.name : "-");

        private static int Triangles(MeshRenderer renderer)
        {
            Mesh mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
            int count = 0;
            for (int part = 0; part < mesh.subMeshCount; part++) count += (int)(mesh.GetIndexCount(part) / 3);
            return count;
        }
    }
}
