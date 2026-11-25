using UnityEngine;
using Steel.Services;
using Steel.Presentation.Input;

namespace Steel.Presentation.Player
{
    public sealed class OrbitCamera : MonoBehaviour
    {
        public Transform Target;
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
            Vector3 focus = Target.position + Vector3.up * 1.5f;
            Vector3 offset = rotation * Vector3.back * host.Layout.CameraDistance;
            float distance = offset.magnitude;
            if (Physics.SphereCast(focus, .2f, offset.normalized, out var hit, distance,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) distance = Mathf.Max(.6f, hit.distance - .15f);
            transform.position = focus + offset.normalized * distance;
            transform.LookAt(focus);
        }
    }
}
