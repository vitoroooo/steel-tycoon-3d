using UnityEngine;

namespace Steel.Presentation.Player
{
    public sealed class WorkerGait : MonoBehaviour
    {
        public Transform LeftArm, RightArm, LeftLeg, RightLeg;
        private Vector3 previous;
        private float phase;
        private void Start() => previous = transform.position;
        private void LateUpdate()
        {
            float speed = (transform.position - previous).magnitude / Mathf.Max(.001f, Time.deltaTime);
            previous = transform.position; phase += Time.deltaTime * 10;
            float swing = Mathf.Sin(phase) * Mathf.Clamp01(speed / 3) * 25;
            LeftArm.localRotation = Quaternion.Euler(swing, 0, -5);
            RightArm.localRotation = Quaternion.Euler(-swing, 0, 5);
            LeftLeg.localRotation = Quaternion.Euler(-swing, 0, 0);
            RightLeg.localRotation = Quaternion.Euler(swing, 0, 0);
        }
    }
}
