using UnityEngine;

namespace Steel.Presentation.Player
{
    public sealed class WorkerAnimation : MonoBehaviour
    {
        private Animator animator;
        private Vector3 previous;
        private void OnEnable() { previous = transform.position; animator = GetComponent<Animator>(); }
        private void LateUpdate()
        {
            float speed = Vector3.Distance(previous, transform.position) / Mathf.Max(Time.deltaTime, .001f);
            animator.SetFloat("Speed", speed, .12f, Time.deltaTime); previous = transform.position;
        }
    }
}
