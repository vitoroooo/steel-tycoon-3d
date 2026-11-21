using UnityEngine;

namespace Steel.Editor.Map
{
    /// <summary>Simple collision for authored models: a few boxes instead of colliding with every triangle.</summary>
    internal static class Solid
    {
        /// <summary>Adds a box in the model's own space; centre and size are in metres.</summary>
        internal static void Box(GameObject model, Vector3 centre, Vector3 size)
        {
            var box = new GameObject("Tumbukan"); box.transform.SetParent(model.transform, false);
            box.transform.localPosition = centre; box.isStatic = true;
            box.AddComponent<BoxCollider>().size = size;
        }

        /// <summary>Puts a model in the city: position, heading and scale.</summary>
        internal static GameObject Stand(GameObject model, Vector3 position, float yaw, float scale = 1)
        {
            model.transform.SetParent(CityGeometry.Root, false);
            model.transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));
            model.transform.localScale = Vector3.one * scale;
            Models.Placement.Settle(model);
            return model;
        }
    }
}
