using UnityEngine;
using UnityEngine.InputSystem;

namespace Steel.Presentation.Input
{
    public sealed class SliceInput : MonoBehaviour, IMoveInput
    {
        private InputAction move, look, interact;
        public bool IsBlocked { get; set; }
        public Vector2 Move => IsBlocked ? Vector2.zero : move.ReadValue<Vector2>();
        public bool Interact => !IsBlocked && interact.WasPressedThisFrame();
        public Vector2 Look
        {
            get
            {
                if (IsBlocked) return Vector2.zero;
                if (Mouse.current != null && Mouse.current.rightButton.isPressed) return look.ReadValue<Vector2>();
                if (Touchscreen.current != null)
                    foreach (var touch in Touchscreen.current.touches)
                        if (touch.press.isPressed && touch.position.ReadValue().x > Screen.width * .5f &&
                            touch.position.ReadValue().y > Screen.height * .4f) return touch.delta.ReadValue();
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
        private void OnDisable() { move.Disable(); look.Disable(); interact.Disable(); }
        private void OnDestroy() { move.Dispose(); look.Dispose(); interact.Dispose(); }
    }
}
