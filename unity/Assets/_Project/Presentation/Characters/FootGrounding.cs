using UnityEngine;

namespace Steel.Presentation.Characters
{
    /// <summary>Stands the visible body on the surface under it. The controller's capsule rides a skin above the
    /// ground and hangs on kerb edges; the feet must not.</summary>
    [DefaultExecutionOrder(-30)]   // before the gait plants the feet on this height
    public sealed class FootGrounding : MonoBehaviour
    {
        private const float ProbeAbove = .5f, Deepest = .45f, Highest = .05f;
        [Tooltip("Metres per second the body may rise or sink to meet the ground.")]
        [SerializeField] private float settleSpeed = 2.5f;
        private SeatedPose seat;
        private float offset;

        /// <summary>Height of the feet above the surface below them; zero when standing properly.</summary>
        public float Gap { get; private set; }

        private void Awake() => seat = GetComponent<SeatedPose>();

        private void LateUpdate()
        {
            if (transform.parent == null || (seat != null && seat.Sitting)) { offset = 0; return; }
            Vector3 body = transform.parent.position;
            bool found = Physics.Raycast(body + Vector3.up * ProbeAbove, Vector3.down, out RaycastHit ground,
                ProbeAbove + Deepest, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            float wanted = found ? Mathf.Clamp(ground.point.y - body.y, -Deepest, Highest) : 0;
            offset = Mathf.MoveTowards(offset, wanted, settleSpeed * Time.deltaTime);
            Vector3 local = transform.localPosition;
            transform.localPosition = new Vector3(local.x, offset, local.z);
            Gap = found ? transform.position.y - ground.point.y : 0;
        }
    }
}
