using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Steel.Editor.Showroom
{
    /// <summary>Renders one view of a subject into a tile of a review sheet.</summary>
    internal sealed class ShowroomCamera
    {
        private const int Supersample = 2;   // stands in for the phone pipeline's 4x MSAA, which software GL cannot render
        private readonly Camera camera;
        private readonly Texture2D sheet;

        /// <param name="sky">Draw the skybox behind the subject instead of the neutral studio grey.</param>
        internal ShowroomCamera(int width, int height, bool sky = false)
        {
            camera = new GameObject("Showroom Camera").AddComponent<Camera>();
            camera.fieldOfView = 30; camera.farClipPlane = 1500;   // the big-map survey looks down from a kilometre up
            camera.clearFlags = sky ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.42f, .44f, .46f);
            camera.gameObject.AddComponent<UniversalAdditionalCameraData>().renderPostProcessing = false;
            sheet = new Texture2D(width, height, TextureFormat.RGB24, false);
        }

        /// <summary>Looks at <paramref name="focus"/> from a yaw/pitch/distance and draws into a tile.</summary>
        internal void Shoot(Vector3 focus, float yaw, float pitch, float distance, RectInt tile)
        {
            camera.transform.rotation = Quaternion.Euler(pitch, yaw + 180, 0);
            camera.transform.position = focus - camera.transform.forward * distance;
            camera.nearClipPlane = Mathf.Max(.05f, distance * .2f);   // keeps depth precise enough to tell asphalt from ground from high up
            var large = RenderTexture.GetTemporary(tile.width * Supersample, tile.height * Supersample, 24);
            var target = RenderTexture.GetTemporary(tile.width, tile.height, 0);
            camera.targetTexture = large;
            camera.Render();
            Graphics.Blit(large, target);
            RenderTexture.ReleaseTemporary(large);
            RenderTexture.active = target;
            sheet.ReadPixels(new Rect(0, 0, tile.width, tile.height), tile.x, tile.y);
            RenderTexture.active = null;
            camera.targetTexture = null;
            RenderTexture.ReleaseTemporary(target);
        }

        internal void Save(string path)
        {
            sheet.Apply();
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            System.IO.File.WriteAllBytes(path, sheet.EncodeToPNG());
            Debug.Log("SHOWROOM_SAVED: " + path);
        }
    }
}
