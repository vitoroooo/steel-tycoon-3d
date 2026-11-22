using System.Collections;
using NUnit.Framework;
using Steel.PlayTests.Support;
using Steel.Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;
using Phase = UnityEngine.InputSystem.TouchPhase;

namespace Steel.PlayTests.Boot
{
    /// <summary>A real touch through the event system, the way a thumb reaches the menu on a phone.</summary>
    public sealed class MenuTouchTests
    {
        private Touchscreen screen;
        private InputSettings.EditorInputBehaviorInPlayMode focus;
        private InputSettings.BackgroundBehavior background;

        [SetUp] public void RouteInputToHeadlessPlayer()
        {
            var settings = InputSystem.settings;
            focus = settings.editorInputBehaviorInPlayMode; background = settings.backgroundBehavior;
            settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        }

        [TearDown] public void RemoveScreen()
        {
            if (screen != null && screen.added) InputSystem.RemoveDevice(screen);
            InputSystem.settings.editorInputBehaviorInPlayMode = focus;
            InputSystem.settings.backgroundBehavior = background;
            BootStage.Close();
        }

        private IEnumerator Tap(Vector2 at)
        {
            InputSystem.QueueStateEvent(screen, new TouchState { touchId = 1, phase = Phase.Began, position = at });
            InputSystem.Update(); yield return null; yield return null;
            InputSystem.QueueStateEvent(screen, new TouchState { touchId = 1, phase = Phase.Ended, position = at });
            InputSystem.Update(); yield return null; yield return null;
        }

        [UnityTest] public IEnumerator TapOnSplashThenOnPlayStartsLoading()
        {
            yield return BootStage.LoadScene();
            var boot = BootStage.Screen;
            screen = InputSystem.AddDevice<Touchscreen>();
            yield return Tap(new Vector2(Screen.width * .5f, Screen.height * .6f));
            Assert.AreEqual(MenuStage.Menu, boot.Flow.Stage, "Any tap leaves the splash");
            Canvas.ForceUpdateCanvases();
            yield return Tap(BootStage.ScreenPoint(boot.Menu.Play));
            Assert.AreEqual(MenuStage.Loading, boot.Flow.Stage, "Thumb on the play button starts the career");
            Assert.IsFalse(boot.Flow.Fresh);
            yield return BootStage.UntilCity();
        }
    }
}
