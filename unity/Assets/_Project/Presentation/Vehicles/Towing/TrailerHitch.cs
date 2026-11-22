using UnityEngine;

namespace Steel.Presentation.Vehicles.Towing
{
    /// <summary>Swings a trailer deck on its kingpin: its axles trail the tractor instead of turning with it.</summary>
    [DefaultExecutionOrder(40)]   // after the tractor has moved this frame
    public sealed class TrailerHitch : MonoBehaviour
    {
        public MotorDriver Driver;
        /// <summary>The deck, pivoted at the kingpin.</summary>
        public Transform Deck;
        /// <summary>Metres from the kingpin back to the middle of the deck's axles.</summary>
        public float Reach = 5;
        /// <summary>The sharpest fold between tractor and deck before the cab would touch the load.</summary>
        public float MaxFold = 62;
        /// <summary>Degrees the deck is folded away from straight; positive when the deck swings to the right.</summary>
        public float Fold { get; private set; }
        private float heading;

        private void OnEnable() => heading = transform.eulerAngles.y;

        private void LateUpdate()
        {
            float tractor = transform.eulerAngles.y;
            float off = Mathf.DeltaAngle(heading, tractor) * Mathf.Deg2Rad;
            // The axle group is dragged by the kingpin: it turns only as far as the pull is sideways to it.
            heading += Driver.View.Speed * Time.deltaTime / Reach * Mathf.Sin(off) * Mathf.Rad2Deg;
            Fold = Mathf.Clamp(Mathf.DeltaAngle(tractor, heading), -MaxFold, MaxFold);
            heading = tractor + Fold;
            Deck.localRotation = Quaternion.Euler(0, Fold, 0);
        }
    }
}
