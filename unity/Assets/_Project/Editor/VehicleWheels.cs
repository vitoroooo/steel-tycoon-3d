using System.Linq;
using UnityEngine;
using Steel.Presentation.Vehicles;

namespace Steel.Editor
{
    internal static class VehicleWheels
    {
        internal static void Create(GameObject root)
        {
            var visual = root.AddComponent<PickupWheels>();
            Transform Pivot(string name)
            {
                var wheel = root.GetComponentsInChildren<MeshRenderer>().First(r => r.name == name);
                visual.Radius = wheel.bounds.extents.y;
                var pivot = new GameObject(name + " / pivot").transform;
                pivot.SetParent(root.transform); pivot.position = wheel.bounds.center;
                pivot.localRotation = Quaternion.identity;
                wheel.transform.SetParent(pivot, true); return pivot;
            }
            visual.FrontLeft = Pivot("FrontWheel_L"); visual.FrontRight = Pivot("FrontWheel_R");
            visual.Rear = Pivot("BackWheels");
        }
    }
}
