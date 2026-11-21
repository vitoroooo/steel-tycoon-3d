using UnityEngine;
using Steel.Services;
using Steel.Presentation.Input;

namespace Steel.Presentation.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMover : MonoBehaviour
    {
        public Transform Visual;
        private CharacterController controller;
        private GameHost host;
        private SliceInput input;
        private float verticalSpeed;
        private bool wasDriving;
        /// <summary>Metres per second squared: feet get up to speed and settle, never snap.</summary>
        private const float Acceleration = 14, Braking = 18;
        private Vector3 planar;
        private void Start()
        {
            controller = GetComponent<CharacterController>();
            host = FindObjectOfType<GameHost>(); input = FindObjectOfType<SliceInput>();
        }
        private void Update()
        {
            bool driving = host.Game.Vehicle.Occupied;
            if (driving || wasDriving)
            {
                controller.enabled = false;
                var view = host.Game.View;
                transform.position = new Vector3(view.PlayerX, .3f, view.PlayerZ);
                controller.enabled = !driving; wasDriving = driving;
                planar = Vector3.zero;
                if (driving) return;
                verticalSpeed = 0;
            }
            Vector2 axis = Vector2.ClampMagnitude(input.Move, 1);
            float yaw = Camera.main.transform.eulerAngles.y;
            Vector3 direction = Quaternion.Euler(0, yaw, 0) * new Vector3(axis.x, 0, axis.y);
            if (controller.isGrounded && verticalSpeed < 0) verticalSpeed = -2;
            verticalSpeed -= 9.81f * Time.deltaTime;
            var cart = host.Game.Cart;
            float speed = cart.Held ? cart.PushSpeed : host.Layout.MoveSpeed;
            Vector3 wanted = direction * speed;
            float rate = wanted.sqrMagnitude > planar.sqrMagnitude ? Acceleration : Braking;
            planar = Vector3.MoveTowards(planar, wanted, rate * Time.deltaTime);
            controller.Move((planar + Vector3.up * verticalSpeed) * Time.deltaTime);
            if (direction.sqrMagnitude > .01f)
                Visual.rotation = Quaternion.Slerp(Visual.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 12);
            host.Game.SetPosition(transform.position.x, transform.position.z);
        }
    }
}
