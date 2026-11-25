using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Steel.Presentation.UI
{
    public sealed class OpenLedger : MonoBehaviour, IPointerClickHandler
    {
        internal Action Open;
        public void OnPointerClick(PointerEventData e) => Open?.Invoke();
    }
}
