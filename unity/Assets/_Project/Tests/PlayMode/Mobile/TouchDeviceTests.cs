using System.Collections;
using NUnit.Framework;
using Steel.PlayTests.Support;
using Steel.Presentation.Input;
using Steel.Presentation.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.TestTools;
using Phase = UnityEngine.InputSystem.TouchPhase;

namespace Steel.PlayTests.Mobile
{
    public sealed class TouchDeviceTests
    {
        private Touchscreen screen;
        private InputSettings.EditorInputBehaviorInPlayMode focus;
        private InputSettings.BackgroundBehavior background;
        [SetUp]
        public void RouteInputToHeadlessPlayer()
        {
            var settings = InputSystem.settings;
            focus = settings.editorInputBehaviorInPlayMode; background = settings.backgroundBehavior;
            settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        }
        private void Send(int id, Phase phase, Vector2 at, Vector2 delta = default) =>
            InputSystem.QueueStateEvent(screen, new TouchState { touchId = id, phase = phase, position = at, delta = delta });
        [TearDown]
        public void RemoveScreen()
        {
            if (screen != null && screen.added) InputSystem.RemoveDevice(screen);
            InputSystem.settings.editorInputBehaviorInPlayMode = focus;
            InputSystem.settings.backgroundBehavior = background;
        }

        [UnityTest]
        public IEnumerator WalkingAndOrbitingTogetherThroughTheEventSystem()
        {
            yield return OwnedFleet.LoadWealthyCity();
            Canvas.ForceUpdateCanvases();
            var input = Object.FindObjectOfType<SliceInput>();
            screen = InputSystem.AddDevice<Touchscreen>();
            var root = Object.FindObjectOfType<CityHud>().SafeRoot;
            Vector2 At(float x, float y) => RectTransformUtility.WorldToScreenPoint(null,
                root.TransformPoint(root.rect.min + Vector2.Scale(root.rect.size, new Vector2(x,y))));
            Vector2 left = At(.21f,.28f), right = At(.75f,.44f);
            Vector2 ahead = At(.21f,.40f), across = At(.62f,.44f);
            var events = EventSystem.current;
            var ray = new List<RaycastResult>();
            events.RaycastAll(new PointerEventData(events){position=left},ray);
            Assert.IsNotEmpty(ray,$"UI at {left}, safe {Screen.safeArea}, root {root.rect}");
            Assert.AreEqual("Area gerak",ray[0].gameObject.name);
            Send(1,Phase.Began,left); Send(2,Phase.Began,right);
            InputSystem.Update();
            yield return null; yield return null;
            float yaw = Camera.main.transform.eulerAngles.y;
            Send(1,Phase.Moved,ahead,ahead-left); Send(2,Phase.Moved,across,across-right);
            InputSystem.Update();
            yield return null; yield return null;
            string state = $"{screen.primaryTouch.position.ReadValue()}, focus={events.isFocused}, module={events.currentInputModule}";
            Assert.Greater(input.Move.y,.5f,"Left touch drives the stick: " + state);
            Assert.Greater(Mathf.Abs(Mathf.DeltaAngle(yaw,Camera.main.transform.eulerAngles.y)),3,"Right touch orbits");
            Send(2,Phase.Ended,across); InputSystem.Update(); yield return null; yield return null;
            Assert.Greater(input.Move.y,.5f,"Lifting camera thumb must not release the stick");
            Send(1,Phase.Ended,ahead); InputSystem.Update(); yield return null; yield return null;
            Assert.AreEqual(Vector2.zero,input.Move);
        }
    }
}
