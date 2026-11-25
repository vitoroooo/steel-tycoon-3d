using System;
using System.Collections.Generic;
using UnityEngine;

namespace Steel.Editor.Map
{
    /// <summary>Cartoon stand-ins for scanned props that cost a phone thousands of triangles each: built from a few primitives.</summary>
    internal static class LightProps
    {
        private static readonly Dictionary<string, Action<Transform, float>> Builders = new Dictionary<string, Action<Transform, float>>
        {
            { "street_lamp_01", Lamp }, { "potted_plant_04", Plant }, { "fern_02", Plant }, { "wooden_crate_01", Crate }
        };

        internal static bool Has(string name) => Builders.ContainsKey(name);

        /// <summary>Builds the stand-in with its foot at position, facing yaw, as tall as asked.</summary>
        internal static GameObject Place(string name, Vector3 position, float height, float yaw, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent == null ? CityGeometry.Root : parent, false);
            go.transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));
            Builders[name](go.transform, height);
            return go;
        }

        /// <summary>A navy post with an arm over the street and a warm lamp head.</summary>
        private static void Lamp(Transform at, float height)
        {
            Material post = CityMaterials.Plain("Tiang lampu", new Color(.12f, .18f, .29f));
            Part(at, PrimitiveType.Cylinder, new Vector3(0, height / 2, 0), new Vector3(.16f, height / 2, .16f), post, true);
            Part(at, PrimitiveType.Cylinder, new Vector3(0, .25f, 0), new Vector3(.34f, .25f, .34f), post, false);
            Part(at, PrimitiveType.Cube, new Vector3(.9f, height - .1f, 0), new Vector3(1.9f, .1f, .1f), post, false);
            Part(at, PrimitiveType.Cube, new Vector3(1.75f, height - .22f, 0), new Vector3(.7f, .16f, .34f),
                CityMaterials.Glow("Lampu jalan", new Color(1, .86f, .55f)), false);
        }

        /// <summary>A terracotta pot under a round green crown.</summary>
        private static void Plant(Transform at, float height)
        {
            float pot = height * .4f, crown = height - pot * .7f;
            Part(at, PrimitiveType.Cylinder, new Vector3(0, pot / 2, 0), new Vector3(pot * .9f, pot / 2, pot * .9f),
                CityMaterials.Plain("Pot tanah liat", new Color(.76f, .4f, .26f)), true);
            Part(at, PrimitiveType.Sphere, new Vector3(0, pot * .7f + crown / 2, 0), new Vector3(crown * .95f, crown, crown * .95f),
                CityMaterials.Plain("Daun pot", new Color(.3f, .62f, .32f)), false);
        }

        /// <summary>A plank crate: a wooden box with two darker battens around it.</summary>
        private static void Crate(Transform at, float height)
        {
            Part(at, PrimitiveType.Cube, new Vector3(0, height / 2, 0), Vector3.one * height,
                CityMaterials.Plain("Peti kayu", new Color(.69f, .48f, .29f)), true);
            foreach (float band in new[] { .22f, .78f })
                Part(at, PrimitiveType.Cube, new Vector3(0, height * band, 0), new Vector3(height * 1.04f, height * .1f, height * 1.04f),
                    CityMaterials.Plain("Bilah peti", new Color(.48f, .3f, .18f)), false);
        }

        private static void Part(Transform at, PrimitiveType type, Vector3 local, Vector3 size, Material material, bool solid)
        {
            GameObject part = CityGeometry.Shape(at.name, type, Vector3.zero, size, material, solid);
            part.transform.SetParent(at, false);
            part.transform.localPosition = local;
        }
    }
}
