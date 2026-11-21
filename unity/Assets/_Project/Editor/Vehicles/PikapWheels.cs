using System.Linq;
using UnityEngine;
using Steel.Presentation.Vehicles;

namespace Steel.Editor.Vehicles
{
    /// <summary>Gives the downloaded pickup a steering pivot and a rolling pivot around each wheel mesh.</summary>
    internal static class PikapWheels
    {
        internal static MotorWheels Create(GameObject root)
        {
            var wheels = root.AddComponent<MotorWheels>();
            Transform Pivot(string name, Transform wheel, Transform parent)
            {
                var pivot = new GameObject(name).transform;
                pivot.SetParent(parent);
                pivot.position = wheel.GetComponent<MeshRenderer>().bounds.center;
                pivot.localRotation = Quaternion.identity;
                return pivot;
            }
            Transform Roll(string mesh, bool steered, out Transform steer)
            {
                Transform wheel = root.GetComponentsInChildren<MeshRenderer>().First(r => r.name == mesh).transform;
                wheels.Radius = wheel.GetComponent<MeshRenderer>().bounds.extents.y;
                steer = steered ? Pivot(mesh + " / kemudi", wheel, root.transform) : null;
                Transform roll = Pivot(mesh + " / putar", wheel, steered ? steer : root.transform);
                wheel.SetParent(roll, true);
                return roll;
            }
            Transform left = Roll("FrontWheel_L", true, out Transform steerLeft);
            Transform right = Roll("FrontWheel_R", true, out Transform steerRight);
            Transform rear = Roll("BackWheels", false, out _);
            wheels.Steered = new[] { steerLeft, steerRight };
            wheels.Rolling = new[] { left, right, rear };
            return wheels;
        }
    }
}
