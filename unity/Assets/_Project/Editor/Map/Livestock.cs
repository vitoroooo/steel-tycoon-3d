using Steel.Editor.Npc;
using UnityEngine;

namespace Steel.Editor.Map
{
    internal static class Livestock
    {
        internal static void Create(Transform player)
        {
            ChickenBuilder.Place(player, false, P(-40, 43), P(-39.4f, 45), P(-40.2f, 47));
            ChickenBuilder.Place(player, true, P(-39.5f, 46.6f), P(-40.5f, 44.8f), P(-40.4f, 48.2f));
            ChickenBuilder.Place(player, true, P(-51.2f, 18), P(-51.6f, 19.3f), P(-51.1f, 21.4f));
            ChickenBuilder.Place(player, true, P(-23.4f, 37), P(-23.3f, 39.4f), P(-23.6f, 42));
        }

        private static Vector3 P(float x, float z) => new Vector3(x, 0, z);
    }
}
