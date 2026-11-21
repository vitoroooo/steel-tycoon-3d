using UnityEngine;

namespace Steel.Editor
{
    internal static class FacadeDetails
    {
        internal static void Street()
        {
            foreach (int x in new[] { -7, 7 })
                foreach (int z in new[] { -15, 12, 33 })
                    ModelLibrary.Place("street_lamp_01", new Vector3(x, .2f, z), 6.5f, x < 0 ? 0 : 180);
            for (int i = 0; i < 4; i++)
                ModelLibrary.Place("modular_chainlink_fence", new Vector3(-24, .15f, -3 + i * 2.5f),
                    2.3f, 90, null, "modular_chainlink_fence");
            ModelLibrary.Place("modular_electric_cables", new Vector3(-21, 3.3f, -4.85f), .6f, 0, null, "cable_bridge_1");
        }
    }
}
