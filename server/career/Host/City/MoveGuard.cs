using System;
using Steel.Services;
using Steel.Services.Remote;

namespace Steel.Career.City
{
    /// <summary>The server's check on where a phone says its player and vehicles are (PRD 12: a phone only shows).
    /// A body moves no further than its top speed allows since its last accepted move; a parked vehicle does not move.</summary>
    public sealed class MoveGuard
    {
        /// <summary>Speed above the rules' own that is still believed, and metres forgiven for a late or lost report.</summary>
        public const double Margin = 1.3, Slack = 10;
        public const string Refusal = "Posisi terlalu jauh dari posisi terakhir.";
        private readonly CityLayout map;
        private double lastAt;

        public MoveGuard(CityLayout map, double clock) { this.map = map; lastAt = clock; }

        /// <summary>The player came back or was put somewhere by the server: time before this buys no distance.</summary>
        public void Rest(double clock) => lastAt = clock;

        /// <summary>Why the call's position is not believed, or null when it is, or when the call does not move anything.</summary>
        public string Refuses(LocalGameService game, RemoteCall call)
        {
            if (call?.args == null) return null;
            bool walking = call.name == "SetPosition" && call.args.Length == 2;
            if (!walking && !(call.name == "ReportVehicle" && call.args.Length == 5)) return null;
            int first = walking ? 0 : 1;
            if (!Number(call.args[first], out double x) || !Number(call.args[first + 1], out double z)) return null;
            return walking ? Walk(game, x, z) : Drive(game, call.args[0], x, z);
        }

        private string Walk(LocalGameService game, double x, double z)
        {
            GameView me = game.View;
            double reach = map.MoveSpeed * Margin * (game.ClockSeconds - lastAt) + Slack;
            if (Far(x, z, me.PlayerX, me.PlayerZ, reach) && Far(x, z, map.Spawn.X, map.Spawn.Z, Slack)) return Refusal;
            lastAt = game.ClockSeconds;
            return null;
        }

        private string Drive(LocalGameService game, string id, double x, double z)
        {
            VehicleView car; SliceLocation home;
            // a vehicle the rules do not know is refused by the call itself
            try { car = game.VehicleOf(id); home = game.VehicleHome(id); }
            catch (ArgumentException) { return null; }
            // a vehicle nobody has placed yet stands where the scene puts it
            if (!car.Heading.HasValue) return null;
            double reach = car.Occupied ? car.MaxSpeed * Margin * (game.ClockSeconds - lastAt) + Slack : Slack;
            if (Far(x, z, car.X, car.Z, reach) && Far(x, z, home.X, home.Z, Slack)) return Refusal;
            if (car.Occupied) lastAt = game.ClockSeconds;
            return null;
        }

        private static bool Far(double x, double z, double fromX, double fromZ, double reach) =>
            !((x - fromX) * (x - fromX) + (z - fromZ) * (z - fromZ) <= reach * reach);

        private static bool Number(string text, out double value)
        {
            try { value = (float)CallText.Read(text, typeof(float)); return true; }
            catch (Exception bad) when (bad is FormatException || bad is OverflowException || bad is ArgumentException)
            { value = 0; return false; }
        }
    }
}
