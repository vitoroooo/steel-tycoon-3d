using System.Globalization;
using UnityEngine;
using Steel.Services;

namespace Steel.Presentation.World.Town
{
    /// <summary>The board at a yard plot's gate. While another player trades from the plot it shows his name and what
    /// he pays per kilogram (PRD 5.2 Papan Harga Beli); otherwise the plot is to let.</summary>
    [RequireComponent(typeof(PlayerPlot))]
    public sealed class PlotBoard : MonoBehaviour
    {
        public TextMesh Sign;
        private static readonly string[] Grades = { "HMS1", "HMS2", "COR", "CAMPUR" };
        private const float Every = 1;
        private GameHost host;
        private PlayerPlot plot;
        private string vacant;
        private float next;

        private void Start()
        {
            host = FindObjectOfType<GameHost>(); plot = GetComponent<PlayerPlot>(); vacant = Sign.text;
        }

        private void Update()
        {
            if (Time.unscaledTime < next) return;
            next = Time.unscaledTime + Every;
            string text = vacant;
            foreach (PlayerYardView yard in host.Game.PlayerYards)
                if (yard.Plot == plot.Number) text = Text(yard);
            if (Sign.text != text) Sign.text = text;
        }

        public static string Text(PlayerYardView yard)
        {
            string Price(int grade) => Grades[grade] + " " + yard.Bids[grade].ToString("N0", CultureInfo.GetCultureInfo("id-ID"));
            return "LAPAK " + yard.Owner.ToUpperInvariant() + "\nBELI/KG " + Price(0) + " · " + Price(1) + "\n" + Price(2) + " · " + Price(3);
        }
    }
}
