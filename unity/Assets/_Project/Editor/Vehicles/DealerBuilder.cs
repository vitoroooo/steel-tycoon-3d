using System.Globalization;
using UnityEngine;
using Steel.Services;

namespace Steel.Editor.Vehicles
{
    /// <summary>The roadside lot where the triseda and the pikap stand for sale, each with its price board.</summary>
    internal static class DealerBuilder
    {
        private static readonly Vector3 Lot = new Vector3(16, 0, 6);
        private static readonly Color SignText = new Color(1, .82f, .45f);
        private const float Width = 11, Depth = 10, WallHeight = 3.2f;

        internal static void Create()
        {
            var concrete = CityMaterials.Surface("concrete_floor_02", new Vector2(3, 3));
            var iron = CityMaterials.Surface("corrugated_iron", new Vector2(4, 1));
            var dark = CityMaterials.Plain("Baja gelap", new Color(.09f, .12f, .14f), .7f, .45f);
            CityGeometry.Box("Dealer / lantai", Lot + Vector3.up * (StreetBuilder.PavementTop - .075f),
                new Vector3(Width, .15f, Depth), concrete);
            DealerDriveway.Create(StreetBuilder.DrivewayZ, StreetBuilder.DrivewayWidth);
            CityGeometry.Box("Dealer / dinding", Lot + new Vector3(0, WallHeight / 2, Depth / 2),
                new Vector3(Width, WallHeight, .2f), iron);
            RoofBuilder.Create(Lot + new Vector3(0, WallHeight + .2f, 2), Width, 6, .8f);
            foreach (float x in new[] { -Width / 2 + .3f, Width / 2 - .3f })
                CityGeometry.Box("Dealer / kolom", Lot + new Vector3(x, WallHeight / 2, -.8f),
                    new Vector3(.14f, WallHeight, .14f), dark);
            CityGeometry.Box("Dealer / papan", Lot + new Vector3(0, 2.5f, Depth / 2 - .2f),
                new Vector3(8, .9f, .12f), dark, false);
            CityGeometry.Sign("DEALER RODA TIGA  /  TUNAI", Lot + new Vector3(0, 2.5f, Depth / 2 - .3f), .22f, SignText);
            var game = new LocalGameService(new ResourceConfigSource());
            PriceBoard(game.VehicleOf(FleetCatalog.Triseda), .4f, dark);
            PriceBoard(game.VehicleOf(FleetCatalog.Pikap), 2.1f, dark);
        }

        /// <summary>A standing board beside the vehicle; the numbers come from the same config the game charges.</summary>
        private static void PriceBoard(VehicleView vehicle, float side, Material frame)
        {
            Vector3 foot = new Vector3(vehicle.X + side, 0, vehicle.Z + 1.5f);   // behind the kerb side, clear of the way out
            CityGeometry.Box("Papan harga / tiang", foot + Vector3.up * .5f, new Vector3(.06f, 1, .06f), frame);
            CityGeometry.Box("Papan harga", foot + Vector3.up * 1.25f, new Vector3(1.5f, .62f, .06f), frame, false);
            string price = (vehicle.Price / 1e6).ToString("0.#", CultureInfo.GetCultureInfo("id-ID"));
            CityGeometry.Sign(vehicle.Label.ToUpperInvariant() + "  Rp " + price + " JUTA\n" +
                VehicleText.Load(vehicle.CapacityKg).ToUpperInvariant(),
                foot + new Vector3(0, 1.25f, -.05f), .1f, SignText);
        }
    }
}
