using Steel.Services;

namespace Steel.Tests.Support
{
    /// <summary>A game whose player can afford the whole dealer lot, for tests about driving.</summary>
    internal static class Wealthy
    {
        internal const double Cash = 200000000;

        internal static IConfigSource Source() =>
            new OverrideConfigSource(new ResourceConfigSource(), "City", "startCash", Cash);

        /// <summary>Walks to the vehicle and buys it; the player is left standing next to it.</summary>
        internal static VehicleView Own(IGameService game, string id)
        {
            VehicleView vehicle = game.VehicleOf(id);
            game.SetPosition(vehicle.X, vehicle.Z);
            game.BuyVehicle(id);
            return game.VehicleOf(id);
        }
    }
}
