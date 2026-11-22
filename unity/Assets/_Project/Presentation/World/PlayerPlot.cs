using UnityEngine;

namespace Steel.Presentation.World
{
    /// <summary>A plot of the shared city that one player can hold. The server decides who; the scene only says where.</summary>
    public sealed class PlayerPlot : MonoBehaviour
    {
        public const string Lapak = "lapak", Factory = "pabrik";
        public string Kind;
        public int Number;
        /// <summary>Frontage and depth in metres; the plot looks down its forward axis at the street.</summary>
        public Vector2 Size;
    }
}
