using System;
using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>Distance and eight-point bearing from the player, e.g. "23 m TL".</summary>
    public static class Compass
    {
        private static readonly string[] Points = {"U","TL","T","TG","S","BD","B","BL"};

        public static string To(GameView view, float x, float z)
        {
            double dx = x - view.PlayerX, dz = z - view.PlayerZ;
            int point = ((int)Math.Round(Math.Atan2(dx, dz) * 4 / Math.PI) + 8) % 8;
            return (int)Math.Ceiling(Math.Sqrt(dx * dx + dz * dz)) + " m " + Points[point];
        }
    }
}
