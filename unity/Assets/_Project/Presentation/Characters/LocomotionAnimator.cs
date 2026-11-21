using UnityEngine;

namespace Steel.Presentation.Characters
{
    /// <summary>Feeds the measured ground speed (m/s) to the idle → walk → run blend.</summary>
    [RequireComponent(typeof(Animator))]
    public sealed class LocomotionAnimator : MonoBehaviour
    {
        /// <summary>Speed is measured over this window so very short frames do not distort it.</summary>
        private const float SampleSeconds = .05f;
        private const float Damping = .12f;
        private static readonly int Speed = Animator.StringToHash("Speed");
        /// <summary>Holds the idle clip while something else carries the avatar, such as a vehicle.</summary>
        public bool Frozen { get; set; }
        private Animator animator;
        private Vector3 sampleStart;
        private float elapsed;
        private float speed;

        private void OnEnable()
        {
            animator = GetComponent<Animator>();
            sampleStart = transform.position;
            elapsed = 0;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed >= SampleSeconds)
            {
                Vector3 travelled = transform.position - sampleStart;
                travelled.y = 0;
                speed = travelled.magnitude / elapsed;
                sampleStart = transform.position;
                elapsed = 0;
            }
            animator.SetFloat(Speed, Frozen ? 0 : speed, Damping, Time.deltaTime);
        }
    }
}
