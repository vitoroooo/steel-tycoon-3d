using UnityEngine;

namespace Steel.Presentation.UI
{
    public sealed partial class CityHud
    {
        private float nextReadout;
        private Transform actor;
        private Vector3 lastPosition;
        public CanvasGroup ActionPad { get; set; }
        private bool DisplayDue()
        {
            if (!actor) actor = FindObjectOfType<CharacterController>().transform;
            var position = actor.position;
            if (Time.unscaledTime < nextReadout && (position - lastPosition).sqrMagnitude < 1) return false;
            lastPosition = position;
            nextReadout = Time.unscaledTime + .1f;
            return true;
        }
    }
}
