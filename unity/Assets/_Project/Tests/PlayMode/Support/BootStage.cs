using System.Collections;
using System.Collections.Generic;
using Steel.Presentation.UI.Boot;
using Steel.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Steel.PlayTests.Support
{
    /// <summary>Opens the start screens for a chosen career without touching the tester's own save files.</summary>
    internal static class BootStage
    {
        internal static BootScreen Screen { get; private set; }

        /// <summary>The Boot scene exactly as the player gets it.</summary>
        internal static IEnumerator LoadScene()
        {
            GameHost.Source = new ResourceConfigSource();
            yield return SceneManager.LoadSceneAsync(BootScreen.Scene);
            yield return null;
            Screen = Object.FindObjectOfType<BootScreen>();
        }

        /// <summary>The same screens, already past the splash, for a career the test describes.</summary>
        internal static IEnumerator OpenMenu(CareerSummary career, GraphicsOption graphics)
        {
            yield return LoadScene();
            if (Screen) Object.DestroyImmediate(Screen.gameObject);
            Screen = new GameObject("Layar Awal Uji").AddComponent<BootScreen>();
            Screen.Show(career, graphics);
            Screen.Flow.Skip();
            yield return null; yield return null;
            Canvas.ForceUpdateCanvases();
        }

        internal static IEnumerator UntilCity()
        {
            float limit = Time.realtimeSinceStartup + 120;
            while (SceneManager.GetActiveScene().name != BootScreen.City && Time.realtimeSinceStartup < limit)
                yield return null;
            yield return null;
        }

        internal static void Close() => GameHost.Source = null;

        /// <summary>The button's rectangle in the safe area's own units, origin at its bottom-left corner.</summary>
        internal static Rect Area(Button button)
        {
            var corners = new Vector3[4];
            ((RectTransform)button.transform).GetWorldCorners(corners);
            RectTransform root = Screen.SafeRoot;
            Vector2 min = (Vector2)root.InverseTransformPoint(corners[0]) - root.rect.min;
            Vector2 max = (Vector2)root.InverseTransformPoint(corners[2]) - root.rect.min;
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        internal static Vector2 ScreenPoint(Button button)
        {
            var corners = new Vector3[4];
            ((RectTransform)button.transform).GetWorldCorners(corners);
            return RectTransformUtility.WorldToScreenPoint(null, (corners[0] + corners[2]) / 2);
        }

        /// <summary>What a finger at the middle of the button would actually press.</summary>
        internal static GameObject Under(Button button)
        {
            var hits = new List<RaycastResult>();
            var events = EventSystem.current;
            events.RaycastAll(new PointerEventData(events) { position = ScreenPoint(button) }, hits);
            return hits.Count == 0 ? null : hits[0].gameObject;
        }
    }
}
