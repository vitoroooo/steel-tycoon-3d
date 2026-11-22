using Steel.Services;

namespace Steel.Editor.Vehicles.Trucks
{
    /// <summary>One heavy truck as measured in tools/blender/kartun/truk/spec.py; forward is +Z here.</summary>
    internal sealed class TruckSpec
    {
        internal const float WheelRadius = .5f, Width = 2.4f, Height = 2.9f;
        /// <summary>The trailer deck: its kingpin from the truck's centre, and how far its axle group trails it.</summary>
        internal const float KingpinZ = -1.825f, DeckReach = 4.9f, DeckLength = 7.4f, DeckNose = 1.1f, DeckTop = 1.42f;

        internal readonly string Id, Model, Name;
        internal readonly float Length;
        /// <summary>Z of every axle, the steered one first.</summary>
        internal readonly float[] Axles;
        internal readonly bool Towing;

        private TruckSpec(string id, string model, string name, float length, bool towing, params float[] axles)
        {
            Id = id; Model = model; Name = name; Length = length; Towing = towing; Axles = axles;
        }

        internal static readonly TruckSpec[] All =
        {
            new TruckSpec(FleetCatalog.Engkel, "EngkelKartun", "Truk engkel", 6.6f, false, 2.3f, -1.9f),
            new TruckSpec(FleetCatalog.Tronton, "TrontonKartun", "Tronton", 9.2f, false, 3.6f, -2.2f, -3.45f),
            new TruckSpec(FleetCatalog.Trailer, "TrailerKartun", "Trailer flat-deck", 6.2f, true, 2.1f, -1.2f, -2.45f)
        };

        /// <summary>Half the distance between the first and the last axle: where the ground is sampled.</summary>
        internal float AxleReach => (Axles[0] - Axles[Axles.Length - 1]) / 2;
        /// <summary>The cab door, measured from the truck's centre.</summary>
        internal float DoorZ => Length / 2 - 1.2f;
    }
}
