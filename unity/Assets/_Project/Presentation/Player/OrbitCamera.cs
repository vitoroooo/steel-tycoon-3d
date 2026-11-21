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
        /// <summary>Seconds the camera takes to catch up, so starts, stops and mounting a vehicle glide.</summary>
        private const float FollowTime = .09f, ReframeTime = .35f;
        private Vector3 focus, focusVelocity;
        private float reach = 1, reachVelocity, height = 1.35f, heightVelocity;
        private bool settled;
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
            reach = Mathf.SmoothDamp(reach, driving ? 1.35f : 1, ref reachVelocity, ReframeTime);
            height = Mathf.SmoothDamp(height, driving ? 1.7f : 1.35f, ref heightVelocity, ReframeTime);
            Vector3 wanted = (driving ? VehicleTarget : Target).position + Vector3.up * height;
            focus = settled ? Vector3.SmoothDamp(focus, wanted, ref focusVelocity, FollowTime) : wanted;
            settled = true;
            Vector3 offset = rotation * Vector3.back * host.Layout.CameraDistance * reach;
            transform.position = CameraCollision.Resolve(focus, offset);
            transform.LookAt(focus);
        }
    }
}
