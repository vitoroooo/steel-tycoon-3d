using UnityEngine;
using Steel.Services;
using Steel.Presentation.Vehicles;

namespace Steel.Editor
{
    internal static class VehicleBuilder
    {
        internal static Transform Create()
        {
            var game = new LocalGameService(new ResourceConfigSource()); var v = game.Vehicle;
            var root = new GameObject("Pikap / pinjaman lapak"); root.layer = 2;
            root.transform.position = new Vector3(v.X, .2f, v.Z);
            var model = ModelLibrary.Place("Pickup", root.transform.position, 1.9f, 0, root.transform);
            VehicleWheels.Create(root);
            foreach (Transform t in root.GetComponentsInChildren<Transform>())
            { t.gameObject.layer = 2; t.gameObject.isStatic = false; }
            var box = root.AddComponent<BoxCollider>(); box.center = Vector3.up;
            box.size = new Vector3(1.85f, 1.65f, 4.4f);
            var body = root.AddComponent<Rigidbody>(); body.isKinematic = true;
            root.AddComponent<PickupDriver>(); return root.transform;
        }
    }
}
