using UnityEngine;
using Steel.Presentation.Vehicles.Haul;

namespace Steel.Editor.Vehicles.Trucks
{
    /// <summary>Puts the visible load on a truck's bed (on the deck, for the trailer); empty until the yard loader fills it.</summary>
    internal static class TruckBed
    {
        private const float Cab = 2.3f, BedTop = 1.3f, FullHeight = 1.5f, Inset = .3f;

        internal static void Attach(GameObject truck, TruckSpec spec)
        {
            Transform bed = spec.Towing ? truck.transform.Find("Deck") : truck.transform;
            float length = spec.Towing ? TruckSpec.DeckLength - 1 : spec.Length - Cab - .4f;
            float middle = spec.Towing ? TruckSpec.DeckNose - TruckSpec.DeckLength / 2 : -spec.Length / 2 + .2f + length / 2;
            GameObject load = CityGeometry.Shape("Muatan", PrimitiveType.Cube, Vector3.zero,
                new Vector3(TruckSpec.Width - Inset * 2, FullHeight, length), CityMaterials.Plain("Rongsok karat", new Color(.62f, .33f, .2f)), false);
            load.isStatic = false; load.transform.SetParent(bed, false);
            load.transform.localPosition = new Vector3(0, BedTop, middle); load.transform.localRotation = Quaternion.identity;
            var view = truck.AddComponent<TruckLoadView>();
            view.Truck = spec.Id; view.Load = load.transform; view.FullHeight = FullHeight;
            view.BedTop = spec.Towing ? TruckSpec.DeckTop : BedTop;
        }
    }
}
