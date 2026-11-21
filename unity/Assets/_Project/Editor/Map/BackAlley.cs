using UnityEngine;

namespace Steel.Editor.Map
{
    /// <summary>The service alley between two rows: a shortcut on foot or with the gerobak, too tight for a pikap.</summary>
    internal static class BackAlley
    {
        /// <summary>Prop, height in metres, and how far from the wall it stands.</summary>
        private static readonly (string model, float height, float fromWall)[] Clutter =
        {
            ("plastic_monobloc_chair_01", .82f, .9f), ("potted_plant_04", .9f, .7f), ("trashbag", .5f, .8f),
            ("wooden_crate_01", .6f, .9f), ("painted_wooden_bench", .8f, .8f), ("fern_02", .55f, .6f),
            ("barrel_03", .95f, .9f), ("utility_box_01", 1.1f, .5f), ("old_tyre", .3f, 1f),
        };
        private const float Spacing = 3.4f;

        /// <summary>Paves the strip between the two aprons and scatters household clutter along both walls.</summary>
        internal static void Create(float x, float south, float north, float width)
        {
            var ground = CityMaterials.Surface("brown_mud_02", new Vector2(2, 14), "Gang belakang tanah",
                new Color(.5f, .48f, .45f));
            CityGeometry.Box("Gang belakang", new Vector3(x, -.11f, (south + north) / 2),
                new Vector3(width, .2f, north - south), ground);
            int count = Mathf.FloorToInt((north - south - 2) / Spacing);
            for (int i = 0; i < count; i++)
            {
                var item = Clutter[(i * 4 + 1) % Clutter.Length];   // stride 4 keeps neighbours different
                int side = i % 2 == 0 ? -1 : 1;
                float z = south + 1.5f + i * Spacing;
                var position = new Vector3(x + side * (width / 2 - item.fromWall), 0, z);
                ModelLibrary.Place(item.model, position, item.height, i * 67 % 360);
            }
        }
    }
}
