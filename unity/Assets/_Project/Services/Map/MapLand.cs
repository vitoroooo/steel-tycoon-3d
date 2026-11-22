using System;
using UnityEngine;

namespace Steel.Services.Map
{
    /// <summary>Answers "whose ground is this?" for any spot in the city, from the baked land picture (one pixel per metre).</summary>
    public sealed class MapLand
    {
        public const int NoPlace = -1;
        private readonly Color32[] pixels;
        private readonly int width, height;
        private readonly MapRect bounds;

        public MapLand(MapPlan plan)
        {
            TextAsset asset = Resources.Load<TextAsset>("Map/SukabajaLahan");
            if (asset == null) throw new InvalidOperationException("Missing land picture: run tools/denah/export.py");
            var picture = new Texture2D(2, 2, TextureFormat.RGB24, false);
            picture.LoadImage(asset.bytes);
            pixels = picture.GetPixels32(); width = picture.width; height = picture.height; bounds = plan.bounds;
            UnityEngine.Object.DestroyImmediate(picture);
        }

        /// <summary>Index into MapPlan.places of the place that owns this spot, or NoPlace on a way, verge or outside.</summary>
        public int PlaceAt(float x, float z) => Inside(x, z, out Color32 pixel) ? pixel.r - 1 : NoPlace;

        /// <summary>True where a street, the railway or the river runs.</summary>
        public bool IsWay(float x, float z) => Inside(x, z, out Color32 pixel) && pixel.g > 0;

        /// <summary>True on a street a vehicle can drive: not the river, not the railway.</summary>
        public bool IsRoad(float x, float z) => Inside(x, z, out Color32 pixel) && pixel.g >= 3;

        // Unity keeps picture rows bottom-up, so row 0 is the south edge.
        private bool Inside(float x, float z, out Color32 pixel)
        {
            int column = Mathf.FloorToInt(x - bounds.west), row = Mathf.FloorToInt(z - bounds.south);
            bool inside = column >= 0 && column < width && row >= 0 && row < height;
            pixel = inside ? pixels[row * width + column] : default;
            return inside;
        }
    }
}
