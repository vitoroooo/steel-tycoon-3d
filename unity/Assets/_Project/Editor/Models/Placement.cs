using UnityEngine;
using Steel.Presentation.World;

namespace Steel.Editor.Models
{
    /// <summary>Finishes a placed model: everything is static except the children its Blender script left free to move.</summary>
    internal static class Placement
    {
        private const string Cloth = "GoyangKain", Leaves = "GoyangDaun", Arm = "Lengan";

        internal static void Settle(GameObject model)
        {
            foreach (Transform part in model.GetComponentsInChildren<Transform>())
            {
                bool moves = part.name.StartsWith("Goyang") || part.name == "Trolley" || part.name == "Load" || part.name == Arm;
                part.gameObject.isStatic = !moves;
                if (part.name.StartsWith(Cloth)) Sway(part, 9, 1.5f, 0);
                else if (part.name.StartsWith(Leaves)) Sway(part, 2.6f, .9f, 1.8f);
                else if (part.name == Arm) part.gameObject.AddComponent<BreakerArm>();
            }
        }

        private static void Sway(Transform part, float degrees, float tempo, float cross)
        {
            var sway = part.gameObject.AddComponent<WindSway>();
            sway.Degrees = degrees; sway.Tempo = tempo; sway.CrossDegrees = cross;
        }
    }
}
