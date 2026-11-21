using UnityEngine;
using Steel.Services;

namespace Steel.Presentation.World
{
    public sealed class PileMarker : MonoBehaviour
    {
        public string Id;
        private GameHost host;
        private void Start() => host = FindObjectOfType<GameHost>();
        private void Update()
        {
            if (host.Game.IsCollected(Id)) gameObject.SetActive(false);
        }
    }
}
