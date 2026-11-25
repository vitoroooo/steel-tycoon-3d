using System;
using UnityEngine;

namespace Steel.Presentation.UI
{
    /// <summary>Calls back twice a second while its panel is shown, so a screen that watches work in progress stays current.</summary>
    internal sealed class Ticker : MonoBehaviour
    {
        private const float Pause = .5f;
        internal Action Every;
        private float wait;

        private void Update()
        {
            if ((wait -= Time.unscaledDeltaTime) > 0) return;
            wait = Pause; Every?.Invoke();
        }
    }
}
