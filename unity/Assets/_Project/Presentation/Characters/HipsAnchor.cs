using UnityEngine;

namespace Steel.Presentation.Characters
{
    /// <summary>
    /// Keeps a walking avatar on the spot. The motion-capture clips carry their forward travel on
    /// the hips bone, while the game moves the character itself, so the travel is cancelled here.
    /// </summary>
    public sealed class HipsAnchor : MonoBehaviour
    {
        [SerializeField] private Transform hips;

        public void Bind(Transform bone) => hips = bone;

        public void Apply()
        {
            Vector3 anchor = transform.position;
            hips.position = new Vector3(anchor.x, hips.position.y, anchor.z);
        }

        private void LateUpdate() => Apply();
    }
}
