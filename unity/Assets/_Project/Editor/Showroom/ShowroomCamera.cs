using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Steel.Editor.Showroom
{
    /// <summary>Renders one view of a subject into a tile of a review sheet.</summary>
    internal sealed class ShowroomCamera
    {
        private readonly Camera camera;
        private readonly Texture2D sheet;

        internal ShowroomCamera(int width, int height)
        {
            camera = new GameObject("Showroom Camera").AddComponent<Camera>();
            camera.fieldOfView = 30;
            camera.nearClipPlane = .05f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.42f, .44f, .46f);
            camera.gameObject.AddComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            sheet = new Texture2D(width, height, TextureFormat.RGB24, false);
        }

        /// <summary>Looks at <paramref name="focus"/> from a yaw/pitch/distance and draws into a tile.</summary>
        internal void Shoot(Vector3 focus, float yaw, float pitch, float distance, RectInt tile)
        {
            camera.transform.rotation = Quaternion.Euler(pitch, yaw + 180, 0);
            camera.transform.position = focus - camera.transform.forward * distance;
            var target = RenderTexture.GetTemporary(tile.width, tile.height, 24, RenderTextureFormat.ARGB32);
            camera.targetTexture = target;
            camera.Render();
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
