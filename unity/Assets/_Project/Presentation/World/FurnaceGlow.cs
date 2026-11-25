using UnityEngine;

namespace Steel.Presentation.World
{
    /// <summary>Makes a light breathe and flicker like molten steel: slow swell plus restless noise.</summary>
    [RequireComponent(typeof(Light))]
    public sealed class FurnaceGlow : MonoBehaviour
    {
        public float Intensity = 9, Swell = .18f, Flicker = .22f;
        private Light glow;
        private float seed;

        private void Awake() { glow = GetComponent<Light>(); seed = transform.position.x * 3.7f + transform.position.z; }

        private void Update()
        {
            float slow = Mathf.Sin(Time.time * .9f + seed) * Swell;
            float fast = (Mathf.PerlinNoise(Time.time * 9, seed) - .5f) * 2 * Flicker;
            glow.intensity = Intensity * (1 + slow + fast);
        }
    }
}
