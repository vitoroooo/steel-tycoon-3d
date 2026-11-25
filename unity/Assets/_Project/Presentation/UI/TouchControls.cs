using Steel.Presentation.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    public static class TouchControls
    {
        private static RectTransform Zone(Transform root, string name, Vector2 min, Vector2 max)
        {
            var area = UiFactory.Rect(name, root, min, Vector2.zero, Vector2.zero);
            area.anchorMax = max; area.offsetMin = area.offsetMax = Vector2.zero;
            area.gameObject.AddComponent<Image>().color = Color.clear;
            return area;
        }
        public static Text Build(CityHud hud, SliceInput controls)
        {
            var root = hud.SafeRoot;
            var sweep = Zone(root,"Usap kamera",new Vector2(.44f,0),Vector2.one);
            sweep.gameObject.AddComponent<OrbitSurface>().Bind(controls);
            var area = Zone(root,"Area gerak",Vector2.zero,new Vector2(.44f,.70f));
            var stick = area.gameObject.AddComponent<FloatingStick>();
            stick.Ring = PadArt.Disc("Pangkal",area,Vector2.zero,144,new Vector2(122,104),true).rectTransform;
            stick.Thumb = PadArt.Disc("Pegangan",stick.Ring,new Vector2(.5f,.5f),57,Vector2.zero,false).rectTransform;
            stick.Bind(controls);

            var anchor = new Vector2(1,0);
            var hit = UiFactory.Panel("Interaksi",root,anchor,new Vector2(140,140),new Vector2(-109,101));
            var mask = hit.GetComponent<Image>(); mask.color = Color.clear;
            hud.ActionPad = hit.gameObject.AddComponent<CanvasGroup>();
            var face = PadArt.Disc("Aksi",hit,new Vector2(.5f,.5f),124,Vector2.zero,false);
            var button = hit.gameObject.AddComponent<Button>(); button.targetGraphic = face;
            var colors = button.colors; colors.pressedColor = new Color(1,.77f,.37f); button.colors = colors;
            button.onClick.AddListener(hud.Act);
            hit.gameObject.AddComponent<ThumbPress>().Face = face.rectTransform;
            var label = UiFactory.Text(face.transform,"",17,new Vector2(112,64),Vector2.zero,TextAnchor.MiddleCenter);
            label.resizeTextForBestFit = true; label.resizeTextMinSize = 13; label.resizeTextMaxSize = 19;
            label.fontStyle = FontStyle.Bold; label.color = new Color(1,.87f,.55f);
            return label;
        }
    }
}
