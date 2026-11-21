using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    internal static class PadArt
    {
        private static Sprite rim, knob;
        internal static Image Disc(string id, Transform parent, Vector2 anchor, int size, Vector2 pos, bool ring)
        {
            var rect = UiFactory.Rect(id, parent, anchor, new Vector2(size,size), pos);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = ring ? (rim ? rim : rim = Make(true)) : (knob ? knob : knob = Make(false));
            image.raycastTarget = false; return image;
        }
        private static Sprite Make(bool ring)
        {
            const int n = 128;
            var tex = new Texture2D(n,n,TextureFormat.RGBA32,false) { wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[n*n];
            var dark = new Color(.055f,.095f,.1f,ring ? .43f : .78f);
            var edge = new Color(.88f,.92f,.85f,.82f);
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float r = new Vector2(x - 63.5f,y - 63.5f).magnitude;
                    var c = Color.Lerp(dark,edge,Mathf.Clamp01(r - 58));
                    c.a *= Mathf.Clamp01(63 - r); pixels[y*n+x] = c;
                }
            tex.SetPixels32(pixels); tex.Apply(false,true);
            return Sprite.Create(tex,new Rect(0,0,n,n),new Vector2(.5f,.5f));
        }
    }
}
