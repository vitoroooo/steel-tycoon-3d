using UnityEngine;

namespace Steel.Editor
{
    internal static class FacadeDetails
    {
        internal static void Create(float x, float z, float height)
        {
            RoofBuilder.Create(new Vector3(x, height + .2f, z), 12.4f, 10.2f, 2);
            SheetMesh.Place(new Vector3(x, 3.6f, z - 5.2f), new Vector3(10.5f, 1, 2),
                Quaternion.Euler(-10, 0, 0));
            ModelLibrary.Place("rollershutter_door", new Vector3(x, .22f, z - 4.75f), 2.7f);
            for (int side = -1; side <= 1; side += 2)
            {
                ModelLibrary.Place("rollershutter_window_01", new Vector3(x + side * 3.6f, .8f, z - 4.8f),
                    1.9f, 0, null, "rollershutter_window_01");
                ModelLibrary.Place("modular_industrial_pipes_01", new Vector3(x + side * 5.3f, .25f, z - 4.8f),
                    2.4f, 0, null, "modular_industrial_pipes_01_pipe01");
                ModelLibrary.Place("modular_electric_cables", new Vector3(x + side * 2.2f, 4, z - 4.9f),
                    .5f, 0, null, "cable_scurve_1");
            }
        }
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
