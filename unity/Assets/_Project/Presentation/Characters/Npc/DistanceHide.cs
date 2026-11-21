using UnityEngine;

namespace Steel.Presentation.Characters.Npc
{
    /// <summary>Switches a bystander's body off when the camera is far away; skinning dozens of unseen people is wasted work on a phone.</summary>
    public sealed class DistanceHide : MonoBehaviour
    {
        public GameObject Body;
        public float Range = 75;
        private const float CheckSeconds = .5f;
        private float wait;

        private void Update()
        {
            if ((wait -= Time.deltaTime) > 0) return;
            wait = CheckSeconds;
            var eye = Camera.main;
            if (!eye) return;
            bool near = (eye.transform.position - transform.position).sqrMagnitude < Range * Range;
            if (Body.activeSelf != near) Body.SetActive(near);
        }
    }
}
