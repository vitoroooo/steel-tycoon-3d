using System.Collections;
using UnityEngine;

namespace Steel.Presentation.World
{
    public sealed class ScreenshotCapture : MonoBehaviour
    {
        private IEnumerator Start()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            int index = System.Array.IndexOf(args, "-screenshot");
            if (index < 0 || index + 1 >= args.Length) yield break;
            yield return new WaitForSeconds(4);
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(args[index + 1]);
        }
    }
}
