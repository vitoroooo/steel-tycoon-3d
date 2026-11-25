using UnityEngine;
using Steel.Services;

namespace Steel.Presentation.World.Gate
{
    /// <summary>Other suppliers' trucks waiting in the lane beside the mill's weighbridge: as many as the gate's queue this game hour.</summary>
    public sealed class GateLine : MonoBehaviour
    {
        private const float Pause = 1;
        /// <summary>Nearest the weighbridge first.</summary>
        public GameObject[] Trucks;
        private GameHost host;
        private float wait;

        private void Start() => host = FindObjectOfType<GameHost>();

        private void Update()
        {
            if ((wait -= Time.deltaTime) > 0) return;
            wait = Pause;
            int queue = host.Game.GateQueue;
            for (int i = 0; i < Trucks.Length; i++) Trucks[i].SetActive(i < queue);
        }
    }
}
