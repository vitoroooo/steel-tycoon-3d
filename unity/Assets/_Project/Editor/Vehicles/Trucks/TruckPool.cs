using UnityEngine;
using Steel.Editor.Toon;

namespace Steel.Editor.Vehicles.Trucks
{
    /// <summary>The truck pool at the north end of the main road: the heavy fleet stands here, nose to the city.</summary>
    internal static class TruckPool
    {
        private const float FacingSouth = 180, BoardZ = 55.4f;

        internal static void Create(Transform player)
        {
            foreach (TruckSpec spec in TruckSpec.All) TruckBuilder.Create(spec, player, FacingSouth);
            var board = CityMaterials.Plain("Pangkalan papan", ToonPalette.Navy);
            CityGeometry.Box("Pangkalan truk / papan", new Vector3(0, 4.6f, BoardZ), new Vector3(9, 1.3f, .2f), board, false);
            foreach (int side in new[] { -1, 1 })
                CityGeometry.Box("Pangkalan truk / tiang", new Vector3(side * 4.2f, 2, BoardZ), new Vector3(.25f, 4, .25f), board);
            CityGeometry.Sign("PANGKALAN TRUK", new Vector3(0, 4.6f, BoardZ - .12f), .42f, ToonPalette.Yellow);
        }
    }
}
