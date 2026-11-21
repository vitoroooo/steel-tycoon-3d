using UnityEngine;
using UnityEngine.EventSystems;

namespace Steel.Presentation.UI
{
    public sealed class OrbitSurface : ThumbSurface
    {
        protected override void Drag(PointerEventData e) => Controls.AddOrbit(e.delta, Screen.safeArea.height);
    }
}
