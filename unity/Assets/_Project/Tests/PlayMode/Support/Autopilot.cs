using UnityEngine;
using Steel.Presentation.Input;

namespace Steel.PlayTests.Support
{
    /// <summary>Steers a vehicle along waypoints through the same stick input a thumb would give.</summary>
    internal sealed class Autopilot : IMoveInput
    {
        private const float Arrive = 2.5f, FullLockDegrees = 35, SharpTurnDegrees = 50;
        private readonly Transform vehicle;
        private readonly Vector3[] route;

        internal Autopilot(Transform vehicle, params Vector3[] route) { this.vehicle = vehicle; this.route = route; }

        /// <summary>How many waypoints the vehicle has passed.</summary>
        internal int Reached { get; private set; }
        internal bool Done => Reached == route.Length;

        public Vector2 Move
        {
            get
            {
                if (Done) return Vector2.zero;
                Vector3 ahead = route[Reached] - vehicle.position;
                ahead.y = 0;
                if (ahead.magnitude < Arrive) { Reached++; return Move; }
                float angle = Vector3.SignedAngle(vehicle.forward, ahead, Vector3.up);
                return new Vector2(Mathf.Clamp(angle / FullLockDegrees, -1, 1),
                    Mathf.Abs(angle) > SharpTurnDegrees ? .45f : .8f);
            }
        }
    }
}
