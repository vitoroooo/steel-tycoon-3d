using UnityEngine;
using Steel.Editor.Buildings;
using Steel.Editor.Models;

namespace Steel.Editor.Map
{
    /// <summary>Stacks of 20 ft containers: a depot east of the main road, and storage wherever a yard needs it.</summary>
    internal static class ContainerDepot
    {
        private const float Length = 6.06f, Width = 2.44f, Height = 2.59f;
        private static readonly Vector3 Lot = new Vector3(30, 0, 26);
        /// <summary>How high each stack stands, row by row from the south; zero leaves a gap to walk through.</summary>
        private static readonly int[][] Rows = { new[] { 2, 3, 1, 2 }, new[] { 1, 0, 2, 3 }, new[] { 3, 2, 2, 0 } };

        internal static void Create()
        {
            var concrete = CityMaterials.Surface("concrete_floor_02", new Vector2(8, 4), "Depo lantai", new Color(.7f, .72f, .72f));
            CityGeometry.Box("Depo kontainer / lantai", Lot + Vector3.down * .02f, new Vector3(31, .15f, 15), concrete);
            for (int row = 0; row < Rows.Length; row++)
                for (int column = 0; column < Rows[row].Length; column++)
                    Stack(Lot + new Vector3((column - 1.5f) * (Length + 1.1f), .05f, (row - 1) * (Width + 2.2f)),
                        90, Rows[row][column], row * 3 + column);
            CityGeometry.Box("Depo / papan", Lot + new Vector3(-13, 3.4f, -7.4f), new Vector3(5, 1, .15f),
                CityMaterials.Plain("Baja gelap", new Color(.09f, .12f, .14f), .7f, .45f), false);
            CityGeometry.Sign("DEPO KONTAINER", Lot + new Vector3(-13, 3.4f, -7.5f), .2f, new Color(1, .8f, .4f));
        }

        /// <summary>Stands containers on top of each other; each is nudged a little, as a reach stacker leaves them.</summary>
        internal static void Stack(Vector3 at, float yaw, int high, int seed)
        {
            for (int level = 0; level < high; level++)
            {
                int pick = seed * 7 + level * 3;
                var box = AuthoredModel.Create("Props/Kontainer", ContainerMaterials.Paint(pick));
                float nudge = (pick % 5 - 2) * .04f;
                Solid.Stand(box, at + new Vector3(nudge, level * Height, -nudge), yaw + (pick % 3 - 1) * 1.5f + (pick % 2) * 180);
                Solid.Box(box, Vector3.up * Height / 2, new Vector3(Width, Height, Length));
            }
        }
    }
}
