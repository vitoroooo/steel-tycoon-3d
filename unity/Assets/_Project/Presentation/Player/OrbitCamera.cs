using UnityEngine;
using Steel.Services;
using Steel.Presentation.Input;

namespace Steel.Presentation.Player
{
    public sealed class OrbitCamera : MonoBehaviour
    {
        public Transform Target;
        /// <summary>Set by the vehicle that is being driven; the camera follows it instead of the player.</summary>
        public Transform VehicleTarget { get; set; }
        private SliceInput input;
        private GameHost host;
        private float yaw, pitch;
        private void Start()
        {
            input = FindObjectOfType<SliceInput>(); host = FindObjectOfType<GameHost>();
            pitch = Mathf.Atan2(host.Layout.CameraHeight - 1.5f, host.Layout.CameraDistance) * Mathf.Rad2Deg;
        }
        private void LateUpdate()
        {
            Vector2 delta = input.Look * host.Layout.LookSensitivity;
            yaw += delta.x; pitch = Mathf.Clamp(pitch - delta.y, 8, 55);
            var rotation = Quaternion.Euler(pitch, yaw, 0);
            bool driving = VehicleTarget != null;
            Vector3 focus = (driving ? VehicleTarget : Target).position + Vector3.up * (driving ? 1.7f : 1.35f);
            Vector3 offset = rotation * Vector3.back * host.Layout.CameraDistance;
            if (driving) offset *= 1.35f;
            transform.position = CameraCollision.Resolve(focus, offset);
            transform.LookAt(focus);
        }
    }
}
