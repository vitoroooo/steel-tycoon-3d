using UnityEngine;

namespace Steel.Services
{
    [DefaultExecutionOrder(-100)]
    public sealed class GameHost : MonoBehaviour
    {
        public IGameService Game { get; private set; }
        public CityLayout Layout { get; private set; }
        private void Awake()
        {
            var source = new ResourceConfigSource();
            Game = new LocalGameService(source); Layout = new CityLayout(source);
        }
        private void Update() => Game.Advance(Time.deltaTime);
    }
}
