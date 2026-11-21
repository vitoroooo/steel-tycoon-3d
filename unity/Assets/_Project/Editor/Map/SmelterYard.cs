using UnityEngine;
using Steel.Editor.Buildings;
using Steel.Editor.Models;
using Steel.Presentation.World;
using Steel.Services;

namespace Steel.Editor.Map
{
    /// <summary>The smelter's grounds: gate and weighbridge where scrap is sold, stock bays under the gantry, product and walls.</summary>
    internal static class SmelterYard
    {
        private const float West = 12.5f, North = -19, South = -71, East = 53, GateHalf = 3.5f;
        private static readonly Vector3 Gantry = new Vector3(23, 0, -51);
        /// <summary>Scrap heaps under the gantry as x, z, heading, scale.</summary>
        private static readonly Vector4[] Heaps = { new Vector4(23, -43, 0, 1), new Vector4(22.5f, -51.5f, 180, .92f), new Vector4(23.5f, -60, 8, 1.05f) };

        internal static void Create(CityLayout layout)
        {
            Vector3 gate = new Vector3(layout.Factory.X, 0, layout.Factory.Z);
            var bridge = Solid.Stand(AuthoredModel.Create("Buildings/PosTimbang", SmelterMaterials.Paint), gate, 0);
            Solid.Box(bridge, new Vector3(4.6f, 1.5f, 0), new Vector3(4.4f, 3, 3.4f));
            CityGeometry.Sign("JEMBATAN TIMBANG", bridge.transform.TransformPoint(4.2f, 3.56f, 2.27f), .15f, new Color(1, .8f, .4f), 180);
            CityGeometry.Sign("TERIMA BESI TUA", bridge.transform.TransformPoint(4.2f, 3.3f, 2.27f), .1f, Color.white, 180);
            var crane = Solid.Stand(AuthoredModel.Create("Props/Derek", SmelterMaterials.Paint), Gantry, 0);
            foreach (int side in new[] { -1, 1 }) Solid.Box(crane, new Vector3(side * 8, 4.5f, 0), new Vector3(.7f, 9, 5.2f));
            var travel = crane.AddComponent<CraneTravel>();
            travel.Trolley = crane.transform.Find("Trolley"); travel.Load = travel.Trolley.Find("Load");
            var block = CityMaterials.Surface("cracked_concrete_wall", new Vector2(6, 1), "Pabrik dinding bay");
            for (int i = 0; i < Heaps.Length; i++)
            {
                var heap = AuthoredModel.Create("Props/TumpukanSkrap", SmelterMaterials.Paint);
                Solid.Stand(heap, new Vector3(Heaps[i].x, 0, Heaps[i].y), Heaps[i].z, Heaps[i].w);
                Solid.Box(heap, Vector3.up, new Vector3(7, 2, 5));
                if (i > 0) CityGeometry.Box("Dinding bay skrap", new Vector3(23, 1, (Heaps[i].y + Heaps[i - 1].y) / 2), new Vector3(12, 2, .4f), block);
            }
            Product(); Walls(gate.x);
        }

        private static void Product()
        {
            for (int i = 0; i < 5; i++)   // strapped bundles waiting for the truck, two high
            {
                var bundle = AuthoredModel.Create("Props/Billet", SmelterMaterials.Paint);
                Solid.Stand(bundle, new Vector3(30.5f, i / 3 * .96f, -22 - (i % 3) * 1.5f + i / 3 * .7f), 90 + (i % 2) * 2);
                Solid.Box(bundle, Vector3.up * .5f, new Vector3(1.1f, 1, 6));
            }
            ContainerDepot.Stack(new Vector3(31, 0, -29.5f), 90, 2, 4);
            ContainerDepot.Stack(new Vector3(14.6f, 0, -36), 0, 1, 9);
        }

        private static void Walls(float gateX)
        {
            var wall = CityMaterials.Surface("cracked_concrete_wall", new Vector2(20, .6f), "Pabrik tembok keliling");
            var dark = CityMaterials.Plain("Baja gelap", new Color(.09f, .12f, .14f), .7f, .45f);
            void Run(float fromX, float fromZ, float toX, float toZ) => CityGeometry.Box("Tembok pabrik",
                new Vector3((fromX + toX) / 2, .6f, (fromZ + toZ) / 2),
                new Vector3(Mathf.Abs(toX - fromX) + .3f, 1.2f, Mathf.Abs(toZ - fromZ) + .3f), wall);
            Run(West, North, West, South); Run(West, North, gateX - GateHalf, North); Run(gateX + GateHalf, North, East, North);
            foreach (int side in new[] { -1, 1 })
                CityGeometry.Box("Tiang gerbang pabrik", new Vector3(gateX + side * GateHalf, 2.9f, North), new Vector3(.5f, 5.8f, .5f), dark);
            CityGeometry.Box("Palang gerbang pabrik", new Vector3(gateX, 5.4f, North), new Vector3(GateHalf * 2 + .5f, .9f, .3f), dark, false);
            CityGeometry.Sign("PT BAJA NUSANTARA", new Vector3(gateX, 5.4f, North + .17f), .22f, new Color(1, .81f, .48f), 180);
            SengFence.Run(new Vector3(East, 0, 4), new Vector3(East, 0, South));
            SengFence.Run(new Vector3(East, 0, South), new Vector3(West, 0, South));
        }
    }
}
