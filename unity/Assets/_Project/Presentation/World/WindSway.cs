using UnityEngine;

namespace Steel.Presentation.World
{
    /// <summary>Swings a hanging or rooted thing in the wind: a steady rhythm whose strength rides slow gusts.</summary>
    public sealed class WindSway : MonoBehaviour
    {
        /// <summary>Hinge direction in the parent's space; washing swings about its line.</summary>
        public Vector3 Axis = Vector3.right;
        public float Degrees = 8, Tempo = 1.3f;
        /// <summary>Smaller rocking about the other horizontal axis, so leaves do not move like a door.</summary>
        public float CrossDegrees;
        private const float GustTempo = .3f;

        private Quaternion rest;
        private Vector3 cross;
        private float phase;

        private void Awake()
        {
            rest = transform.localRotation;
            cross = Vector3.Cross(Vector3.up, Axis).normalized;
            Vector3 p = transform.position;
            phase = p.x * 1.3f + p.z * .7f + p.y * 2.1f;   // neighbours fall out of step with each other
        }

        // Nothing moves while nobody can see it.
        private void OnBecameVisible() => enabled = true;
        private void OnBecameInvisible() => enabled = false;

        private void Update()
        {
            float t = Time.time;
            float gust = Mathf.PerlinNoise(t * GustTempo + phase, phase * .37f);   // 0 calm .. 1 blowing
            float swing = (Mathf.Sin(t * Tempo + phase) * (.3f + gust) + (gust - .4f)) * Degrees;
            float rock = Mathf.Sin(t * Tempo * .73f + phase * 1.9f) * (.3f + gust) * CrossDegrees;
            transform.localRotation = Quaternion.AngleAxis(swing, Axis) * Quaternion.AngleAxis(rock, cross) * rest;
        }
    }
}
