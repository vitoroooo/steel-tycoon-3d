using UnityEngine;
using Steel.Services;

namespace Steel.Presentation.World
{
    public sealed class PileMarker : MonoBehaviour
    {
        public string Id;
        private GameHost host;
        private bool hidden;
        private void Start() => host = FindObjectOfType<GameHost>();
        private void Update()
        {
            bool collected = host.Game.IsCollected(Id);
            if (collected == hidden) return;
            hidden = collected;
            foreach (Transform part in transform) part.gameObject.SetActive(!hidden);
        }
    }
}
