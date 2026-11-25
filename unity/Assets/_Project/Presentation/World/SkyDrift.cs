using UnityEngine;

namespace Steel.Presentation.World
{
    /// <summary>Turns the sun slowly so shadows creep round; the painted clouds drift on their own in the sky shader.</summary>
    [RequireComponent(typeof(Light))]
    public sealed class SkyDrift : MonoBehaviour
    {
        [Tooltip("Degrees per second; the default takes about three hours for a full turn.")]
        [SerializeField] private float speed = .035f;

        private void Start() => gameObject.AddComponent<LocalRain>();

        private void Update() => transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.World);
    }
}
