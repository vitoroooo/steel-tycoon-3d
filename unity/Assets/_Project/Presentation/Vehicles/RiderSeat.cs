using UnityEngine;
using Steel.Presentation.Characters;

namespace Steel.Presentation.Vehicles
{
    /// <summary>Puts the driver on the vehicle: seated in the open on a saddle, or out of sight in a cab.</summary>
    [DefaultExecutionOrder(30)]   // before SeatedPose and ArmReach pose the avatar this frame
    public sealed class RiderSeat : MonoBehaviour
    {
        public MotorDriver Driver;
        /// <summary>The avatar, and the player object it goes back to when the driver gets off.</summary>
        public Transform Rider, RiderHome;
        /// <summary>A cab hides the driver; leave the seat fields empty for one.</summary>
        public bool Enclosed;
        /// <summary>Hip joints, ankles and wrists of a seated rider. Mount is the ground under the saddle.</summary>
        public Transform Mount, Hips, FootLeft, FootRight, GripLeft, GripRight;
        private bool wasOccupied;
        private Vector3 homeOffset;

        private void LateUpdate()
        {
            bool occupied = Driver.View.Occupied;
            if (occupied == wasOccupied) return;
            wasOccupied = occupied;
            if (Enclosed) Rider.gameObject.SetActive(!occupied);
            else if (occupied) Sit();
            else Stand();
        }

        private void Sit()
        {
            homeOffset = Rider.localPosition;
            Rider.SetParent(Mount, false);
            Rider.localPosition = Vector3.zero; Rider.localRotation = Quaternion.identity;
            Rider.GetComponent<SeatedPose>().Sit(Hips, FootLeft, FootRight);
            Rider.GetComponent<ArmReach>().Hold(GripLeft, GripRight);
        }

        private void Stand()
        {
            Rider.GetComponent<SeatedPose>().Stand();
            Rider.GetComponent<ArmReach>().Release();
            Rider.SetParent(RiderHome, false);
            Rider.localPosition = homeOffset;
            Rider.localRotation = Quaternion.Euler(0, transform.eulerAngles.y - RiderHome.eulerAngles.y, 0);
        }
    }
}
