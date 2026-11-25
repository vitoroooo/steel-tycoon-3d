using UnityEngine;
using Steel.Services;

namespace Steel.Presentation.World.Street
{
    /// <summary>Shows a door's goods while there is something to buy and clears them away once it is sold out.</summary>
    public sealed class SellerMarker : MonoBehaviour
    {
        private const float Pause = .5f;
        public string Id;
        private GameHost host;
        private bool shown = true;
        private float wait;

        private void Start() => host = FindObjectOfType<GameHost>();

        private void Update()
        {
            if ((wait -= Time.deltaTime) > 0) return;
            wait = Pause;
            bool stocked = host.Game.SellerStocked(Id);
            if (stocked == shown) return;
            shown = stocked;
            foreach (Transform piece in transform) piece.gameObject.SetActive(shown);
            GetComponent<Collider>().enabled = shown;
        }
    }
}
