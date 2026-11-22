using UnityEngine;
using Steel.Services;

namespace Steel.Presentation.World.Risk
{
    /// <summary>The yard's gate as the street sees it: the gang standing outside while it waits for an answer, and the guards on duty inside.</summary>
    public sealed class GateTrouble : MonoBehaviour
    {
        private const float Pause = 1;
        public GameObject[] Thugs, Guards;
        private GameHost host;
        private float wait;

        private void Start() { host = FindObjectOfType<GameHost>(); Show(0, 0); }

        private void Update()
        {
            if ((wait -= Time.deltaTime) > 0) return;
            wait = Pause;
            RiskView risk = host.Game.Risk;
            int posted = 0;
            for (int cell = 0; cell < risk.Guards.Length; cell++) if (risk.Guards[cell] > 0 && !risk.Fainted[cell]) posted++;
            Show(risk.Thugs, risk.Open ? posted : 0);
        }

        private void Show(int thugs, int guards)
        {
            for (int i = 0; i < Thugs.Length; i++) Thugs[i].SetActive(i < thugs);
            for (int i = 0; i < Guards.Length; i++) Guards[i].SetActive(i < guards);
        }
    }
}
