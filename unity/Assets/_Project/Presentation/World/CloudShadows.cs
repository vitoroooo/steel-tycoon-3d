using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Steel.Presentation.World
{
    /// <summary>Slides the sun's cloud-shadow cookie across the ground, so the light on the city keeps changing.</summary>
    [RequireComponent(typeof(UniversalAdditionalLightData))]
    public sealed class CloudShadows : MonoBehaviour
    {
        [Tooltip("Wind over the ground in metres per second, along the cookie's two axes.")]
        [SerializeField] private Vector2 wind = new Vector2(1.6f, .7f);

        private UniversalAdditionalLightData data;

        private void Awake() => data = GetComponent<UniversalAdditionalLightData>();

        private void Update()
        {
            Vector2 size = data.lightCookieSize;
            Vector2 offset = data.lightCookieOffset + wind * Time.deltaTime;
            // Wrapping at one tile keeps the offset small; the cookie repeats, so nothing jumps.
            data.lightCookieOffset = new Vector2(Mathf.Repeat(offset.x, size.x), Mathf.Repeat(offset.y, size.y));
        }
    }
}
