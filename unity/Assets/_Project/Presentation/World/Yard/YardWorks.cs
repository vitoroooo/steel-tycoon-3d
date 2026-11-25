using UnityEngine;
using Steel.Services;

namespace Steel.Presentation.World.Yard
{
    /// <summary>The player's yard as the city sees it: heaps that grow with the stock, and a loader that shuttles while a truck is loaded.</summary>
    public sealed class YardWorks : MonoBehaviour
    {
        private const float Pause = .5f, FullKg = 60000, FullSize = 5.5f, Pace = 2.2f;
        public Transform[] Heaps;
        public Transform Loader;
        /// <summary>Where the loader waits by the heaps and where it tips into the truck, on this lot.</summary>
        public Vector3 Rest, Dock;
        private GameHost host;
        private float wait, trip;
        private bool loading;

        private void Start() { host = FindObjectOfType<GameHost>(); Loader.localPosition = Rest; Show(false, new double[Heaps.Length]); }

        private void Update()
        {
            if ((wait -= Time.deltaTime) <= 0)
            {
                wait = Pause;
                HaulView dock = host.Game.YardDock;
                loading = dock.Loading;
                Show(host.Game.View.Caste >= 2, dock.Stock);
            }
            if (!Loader.gameObject.activeSelf) return;
            trip = loading ? trip + Time.deltaTime / Pace : 0;
            Vector3 from = Loader.localPosition, to = loading ? Vector3.Lerp(Rest, Dock, Mathf.PingPong(trip, 1)) : Rest;
            Loader.localPosition = Vector3.MoveTowards(from, to, 4 * Time.deltaTime);
            if ((to - from).sqrMagnitude > .0004f)
                Loader.localRotation = Quaternion.Slerp(Loader.localRotation, Quaternion.LookRotation(to - from), 6 * Time.deltaTime);
        }

        /// <summary>A heap's volume follows its tonnage, so twice the stock is plainly a bigger heap.</summary>
        private void Show(bool open, double[] stock)
        {
            Loader.gameObject.SetActive(open);
            for (int grade = 0; grade < Heaps.Length; grade++)
            {
                float size = FullSize * Mathf.Pow((float)(stock[grade] / FullKg), 1 / 3f);
                Heaps[grade].gameObject.SetActive(open && size > .2f);
                Heaps[grade].localScale = new Vector3(size * 1.5f, size, size * 1.2f);
            }
        }
    }
}
