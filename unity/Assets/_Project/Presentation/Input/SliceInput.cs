using UnityEngine;
using UnityEngine.InputSystem;

namespace Steel.Presentation.Input
{
    public sealed class SliceInput : MonoBehaviour, IMoveInput
    {
        private InputAction move, look, interact;
        private Vector2 stick, orbit;
        private bool blocked;
        public event System.Action ResetTouches;
        public bool IsBlocked
        {
            get => blocked;
            set { if (value && !blocked) ClearTouch(); blocked = value; }
        }
        public Vector2 Move => IsBlocked ? Vector2.zero : stick != Vector2.zero ? stick : move.ReadValue<Vector2>();
        public void SetStick(Vector2 value) => stick = IsBlocked ? Vector2.zero : Vector2.ClampMagnitude(value, 1);
        public void AddOrbit(Vector2 pixels, float height)
        {
            if (!IsBlocked && height > 0) orbit += pixels * (175f / height);
        }
        public Vector2 TakeOrbit() { var delta = orbit; orbit = Vector2.zero; return delta; }
        private void ClearTouch() { stick = orbit = Vector2.zero; ResetTouches?.Invoke(); }
        private void OnApplicationFocus(bool focused) { if (!focused) ClearTouch(); }
        private void OnApplicationPause(bool paused) { if (paused) ClearTouch(); }
        public bool Interact => !IsBlocked && interact.WasPressedThisFrame();
        public Vector2 Look
        {
            get
            {
                if (IsBlocked) return Vector2.zero;
                if (Mouse.current != null && Mouse.current.rightButton.isPressed) return look.ReadValue<Vector2>();
                return Vector2.zero;
            }
        }
        private void Awake()
        {
            move = new InputAction("Jalan", InputActionType.Value);
            move.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            move.AddBinding("<Gamepad>/leftStick");
            look = new InputAction("Kamera", InputActionType.Value, "<Mouse>/delta");
            interact = new InputAction("Interaksi", InputActionType.Button, "<Keyboard>/e");
            interact.AddBinding("<Gamepad>/buttonSouth");
        }
        private void OnEnable() { move.Enable(); look.Enable(); interact.Enable(); }
        private void OnDisable() { ClearTouch(); move.Disable(); look.Disable(); interact.Disable(); }
        private void OnDestroy() { move.Dispose(); look.Dispose(); interact.Dispose(); }
    }
}
