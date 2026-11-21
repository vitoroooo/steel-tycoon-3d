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
        private void Start()
        {
            controller = GetComponent<CharacterController>();
            host = FindObjectOfType<GameHost>(); input = FindObjectOfType<SliceInput>();
        }
        private void Update()
        {
            Vector2 axis = Vector2.ClampMagnitude(input.Move, 1);
            float yaw = Camera.main.transform.eulerAngles.y;
            Vector3 direction = Quaternion.Euler(0, yaw, 0) * new Vector3(axis.x, 0, axis.y);
            if (controller.isGrounded && verticalSpeed < 0) verticalSpeed = -2;
            verticalSpeed -= 9.81f * Time.deltaTime;
            controller.Move((direction * host.Layout.MoveSpeed + Vector3.up * verticalSpeed) * Time.deltaTime);
            if (direction.sqrMagnitude > .01f)
                Visual.rotation = Quaternion.Slerp(Visual.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 12);
            host.Game.SetPosition(transform.position.x, transform.position.z);
        }
    }
}
