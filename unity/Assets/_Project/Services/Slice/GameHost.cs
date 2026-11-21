using UnityEngine;

namespace Steel.Services
{
    [DefaultExecutionOrder(-100)]
    public sealed class GameHost : MonoBehaviour
    {
        public IGameService Game { get; private set; }
        public CityLayout Layout { get; private set; }
        /// <summary>Config to boot from; left null the game reads Resources/Config.</summary>
        public static IConfigSource Source { get; set; }

        private void Awake()
        {
            var source = Source ?? new ResourceConfigSource();
            Game = new LocalGameService(source); Layout = new CityLayout(source);
        }
        private void Update() => Game.Advance(Time.deltaTime);
    }
}
