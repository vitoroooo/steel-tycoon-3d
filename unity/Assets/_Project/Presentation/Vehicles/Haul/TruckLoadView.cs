using UnityEngine;
using Steel.Services;

namespace Steel.Presentation.Vehicles.Haul
{
    /// <summary>The scrap on a truck's bed: it rises as the loader fills it and sinks as the mill's crane empties it.</summary>
    public sealed class TruckLoadView : MonoBehaviour
    {
        private const float Pause = .5f;
        public string Truck;
        public Transform Load;
        public float FullHeight, BedTop;
        private GameHost host;
        private float wait;

        private void Start() { host = FindObjectOfType<GameHost>(); Load.gameObject.SetActive(false); }

        private void Update()
        {
            if ((wait -= Time.deltaTime) > 0) return;
            wait = Pause;
            float share = Mathf.Clamp01((float)host.Game.BedShare(Truck));
            Load.gameObject.SetActive(share > .01f);
            Vector3 size = Load.localScale; size.y = Mathf.Max(.05f, FullHeight * share); Load.localScale = size;
            Vector3 at = Load.localPosition; at.y = BedTop + size.y / 2; Load.localPosition = at;
        }
    }
}
