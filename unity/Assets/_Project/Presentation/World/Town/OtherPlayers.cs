using System.Linq;
using UnityEngine;
using Steel.Services;
using Steel.Services.Remote;

namespace Steel.Presentation.World.Town
{
    /// <summary>The shared city (PRD 1): shows the players the server says are near, each as a walking figure with his name.</summary>
    public sealed class OtherPlayers : MonoBehaviour
    {
        public Ghost[] Ghosts;
        private GameHost host;
        private RemotePlayer[] shown;

        private void Start()
        {
            host = FindObjectOfType<GameHost>();
            foreach (Ghost ghost in Ghosts) ghost.Hide();
        }

        private void Update()
        {
            RemotePlayer[] others = host.Others;
            if (ReferenceEquals(others, shown)) return;   // the server has said nothing new
            shown = others;
            foreach (Ghost ghost in Ghosts)
                if (ghost.Player != null && others.All(other => other.name != ghost.Player)) ghost.Hide();
            foreach (RemotePlayer other in others)
            {
                Ghost ghost = Ghosts.FirstOrDefault(g => g.Player == other.name) ?? Ghosts.FirstOrDefault(g => g.Player == null);
                if (ghost != null) ghost.Show(other, (float)RemoteLink.Beat);
            }
        }
    }
}
