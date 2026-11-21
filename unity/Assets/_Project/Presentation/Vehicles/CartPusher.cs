using UnityEngine;
using Steel.Presentation.Characters;
using Steel.Services;

namespace Steel.Presentation.Vehicles
{
    /// <summary>Keeps the gerobak ahead of the player while its push bar is held.</summary>
    public sealed class CartPusher : MonoBehaviour
    {
        /// <summary>Distance from the player's feet to the axle when holding the grips.</summary>
        public float Reach = 1.72f;
        public float WheelRadius = .33f;
        public Transform Player, PlayerVisual, WheelLeft, WheelRight, GripLeft, GripRight;
        /// <summary>Lifting the bar tips the cart forward and takes the resting legs off the ground.</summary>
        public const float LiftDegrees = 2.5f;
        private const float TurnRate = 9;
        private GameHost host;
        private Collider parkedCollider;
        private ArmReach arms;
        private bool wasHeld;
        private float yaw, roll;

        private void Start()
        {
            host = FindObjectOfType<GameHost>();
            parkedCollider = GetComponent<Collider>();
            arms = PlayerVisual.GetComponent<ArmReach>();
            yaw = transform.eulerAngles.y;
            transform.position = OnGround(transform.position);
        }

        private void LateUpdate()
        {
            bool held = host.Game.Cart.Held;
            if (held != wasHeld) Switch(held);
            if (!held) return;
            yaw = Mathf.LerpAngle(yaw, PlayerVisual.eulerAngles.y, Time.deltaTime * TurnRate);
            Vector3 ahead = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
            Vector3 target = OnGround(Player.position + ahead * Reach);
            float travelled = Vector3.Dot(target - transform.position, ahead);
            transform.SetPositionAndRotation(target, Quaternion.Euler(LiftDegrees, yaw, 0));
            roll = (roll + travelled / WheelRadius * Mathf.Rad2Deg) % 360;
            WheelLeft.localRotation = WheelRight.localRotation = Quaternion.Euler(roll, 0, 0);
        }

        private void Switch(bool held)
        {
            wasHeld = held;
            parkedCollider.enabled = !held;   // a held cart must not shove its own pusher
            if (held) arms.Hold(GripLeft, GripRight);
            else
            {
                arms.Release();
                transform.rotation = Quaternion.Euler(0, yaw, 0);   // back onto its legs
            }
        }

        /// <summary>Drops a point onto the street below it so the wheels follow kerbs and slopes.</summary>
        private static Vector3 OnGround(Vector3 point)
        {
            Vector3 above = point + Vector3.up * 1.5f;
            return Physics.Raycast(above, Vector3.down, out var hit, 4, Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore) ? hit.point : point;
        }
    }
}
