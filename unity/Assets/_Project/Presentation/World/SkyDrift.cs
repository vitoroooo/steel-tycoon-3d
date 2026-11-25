using UnityEngine;

namespace Steel.Presentation.World
{
    /// <summary>Turns the sky picture and the sun together, slowly, so clouds travel and shadows creep round.</summary>
    [RequireComponent(typeof(Light))]
    public sealed class SkyDrift : MonoBehaviour
    {
        private static readonly int Rotation = Shader.PropertyToID("_Rotation");

        [Tooltip("Degrees per second; the default takes about three hours for a full turn.")]
        [SerializeField] private float speed = .035f;

        private Material sky;

        private void Start()
        {
            gameObject.AddComponent<LocalRain>();
            if (RenderSettings.skybox == null) { enabled = false; return; }
            sky = new Material(RenderSettings.skybox);   // a copy, so play mode never edits the asset
            RenderSettings.skybox = sky;
        }

        private void Update()
        {
            float step = speed * Time.deltaTime;
            transform.Rotate(Vector3.up, step, Space.World);
            // The panoramic shader turns the other way round from Transform.Rotate, hence the minus.
            sky.SetFloat(Rotation, Mathf.Repeat(sky.GetFloat(Rotation) - step, 360));
        }

        private void OnDestroy()
        {
            if (sky != null) Destroy(sky);
        }
    }
}
