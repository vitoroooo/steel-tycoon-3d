using System;
using UnityEngine;

namespace Steel.Editor.Buildings
{
    /// <summary>Paints Kontainer.fbx; each box gets one of the faded shipping-line colours.</summary>
    internal static class ContainerMaterials
    {
        private static readonly (string name, Color paint)[] Lines =
        {
            ("marun", new Color(.5f, .17f, .14f)), ("biru", new Color(.16f, .3f, .5f)), ("hijau", new Color(.2f, .4f, .3f)),
            ("jingga", new Color(.78f, .38f, .12f)), ("abu", new Color(.58f, .6f, .6f)),
        };

        internal static Func<string, Material> Paint(int index)
        {
            var line = Lines[index % Lines.Length];
            return slot =>
            {
                switch (slot)
                {
                    case "paint": return CityMaterials.Surface("container_side", Vector2.one, "Kontainer " + line.name, line.paint);
                    case "roof": return CityMaterials.Surface("container_side", Vector2.one, "Kontainer atap " + line.name, line.paint * .8f);
                    case "frame": return CityMaterials.Plain("Kontainer rangka " + line.name, line.paint * .7f, .4f, .4f);
                    case "steel": return CityMaterials.Plain("Kontainer baja", new Color(.12f, .12f, .13f), .6f, .4f);
                    case "plate": return CityMaterials.Plain("Kontainer pelat data", new Color(.85f, .85f, .82f), .1f, .4f);
                    case "seal": return CityMaterials.Plain("Kontainer karet", new Color(.05f, .05f, .05f), 0, .3f);
                    default: throw new ArgumentException("Unknown container material slot: " + slot);
                }
            };
        }
    }
}
