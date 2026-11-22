using UnityEngine;

namespace Steel.Presentation.UI.Boot
{
    /// <summary>Cross-fades one start screen. Touch follows Visible at once; only the alpha is animated.</summary>
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class Fader : MonoBehaviour
    {
        private const float PerSecond = 5;
        private CanvasGroup group;
        private bool visible;

        public bool Visible
        {
            get => visible;
            set
            {
                visible = value; Group.blocksRaycasts = Group.interactable = value;
                if (value) gameObject.SetActive(true);
            }
        }
        /// <summary>True once the screen has finished fading in.</summary>
        public bool Shown => visible && Group.alpha >= 1;
        private CanvasGroup Group => group ? group : group = GetComponent<CanvasGroup>();

        internal static Fader On(RectTransform screen, bool visible)
        {
            screen.gameObject.AddComponent<CanvasGroup>();
            var fader = screen.gameObject.AddComponent<Fader>();
            fader.Visible = visible; fader.Group.alpha = visible ? 1 : 0;
            screen.gameObject.SetActive(visible);
            return fader;
        }

        private void Update()
        {
            Group.alpha = Mathf.MoveTowards(Group.alpha, visible ? 1 : 0, PerSecond * Time.unscaledDeltaTime);
            if (!visible && Group.alpha <= 0) gameObject.SetActive(false);
        }
    }
}
