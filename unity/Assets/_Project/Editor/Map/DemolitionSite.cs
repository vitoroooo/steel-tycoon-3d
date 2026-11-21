using UnityEngine;
using Steel.Editor.Buildings;
using Steel.Editor.Models;

namespace Steel.Editor.Map
{
    /// <summary>Proyek Bongkaran in the south-west: a half-demolished building behind a seng fence and a gate. DemolitionYard fills the lot.</summary>
    internal static class DemolitionSite
    {
        private static readonly Vector3 Building = new Vector3(-30, 0, -42);
        private const float West = -53, North = -19.5f, South = -71, GateWest = -24, GateEast = -18, SmelterWall = 12.5f;
        private const float BayX = 4.5f, BayY = 5, Storey = 3.2f, Column = .4f;   // as in tools/blender/bongkaran/dims.py

        internal static void Create()
        {
            var wreck = Solid.Stand(AuthoredModel.Create("Buildings/Bongkaran", DemolitionMaterials.Paint), Building, 0);
            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 2; j++)
                {
                    float height = Mathf.Max(1, 3 - i) * Storey;
                    Solid.Box(wreck, new Vector3(-6.75f + i * BayX, height / 2, -5 + j * BayY), new Vector3(Column, height, Column));
                }
            foreach (int face in new[] { -1, 1 })   // the ground-floor walls of the two bays still standing
                Solid.Box(wreck, new Vector3(-2.25f, 1, face * 5), new Vector3(9, 2, .2f));
            Solid.Box(wreck, new Vector3(5.5f, .5f, 0), new Vector3(6.5f, 1, 8));   // the rubble mound: walk round, not through
            Gate();
            DemolitionYard.Create();
            SengFence.Run(new Vector3(West, 0, South), new Vector3(West, 0, North));
            SengFence.Run(new Vector3(West, 0, North), new Vector3(GateWest, 0, North));
            SengFence.Run(new Vector3(GateEast, 0, North), new Vector3(-9, 0, North));
            SengFence.Run(new Vector3(SmelterWall, 0, South), new Vector3(West, 0, South));
        }

        private static void Gate()
        {
            var dark = CityMaterials.Plain("Baja gelap", new Color(.09f, .12f, .14f), .7f, .45f);
            Vector3 board = new Vector3((GateWest + GateEast) / 2, 0, North);
            foreach (float x in new[] { GateWest, GateEast })
                CityGeometry.Box("Tiang gerbang proyek", new Vector3(x, 2, North), new Vector3(.2f, 4, .2f), dark);
            CityGeometry.Box("Papan proyek", board + Vector3.up * 3.6f, new Vector3(GateEast - GateWest + .2f, .9f, .12f), dark, false);
            CityGeometry.Sign("PROYEK BONGKARAN", board + new Vector3(0, 3.75f, .08f), .2f, new Color(1, .8f, .4f));
            CityGeometry.Sign("AWAS RERUNTUHAN  -  WAJIB HELM", board + new Vector3(0, 3.4f, .08f), .1f, Color.white);
        }
    }
}
