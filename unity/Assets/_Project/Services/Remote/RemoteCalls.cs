using System.Collections.Generic;

namespace Steel.Services.Remote
{
    /// <summary>How each IGameService method travels to the career server (PRD 12: the server decides, the phone shows).
    /// A method listed nowhere here is a command: sent to the server and decided there.</summary>
    public static class RemoteCalls
    {
        /// <summary>The server keeps its own clock; a phone never tells it how much time passed.</summary>
        public static readonly HashSet<string> Time = new HashSet<string> { "Advance", "CatchUp" };

        /// <summary>Where the player and the trucks are. Sent along with the next command instead of every frame.</summary>
        public static readonly HashSet<string> Motion = new HashSet<string> { "SetPosition", "ReportVehicle" };

        /// <summary>Change nothing, so the phone answers them itself. server/career/Tests checks each one leaves the career as it was.</summary>
        public static readonly HashSet<string> Queries = new HashSet<string>
        {
            "BedShare", "Buyer", "CreatePile", "CrewInfo", "Finances", "GovernmentOrder", "IsCollected", "MaintenanceInfo",
            "MillInfo", "NeedsRespawn", "OwnScanner", "Quote", "Route", "Satellite", "ScannerCost", "Seller", "SellerStocked",
            "VehicleOf", "Weather", "YardInfo"
        };

        public static bool IsCommand(string method) =>
            !Time.Contains(method) && !Motion.Contains(method) && !Queries.Contains(method);

        /// <summary>What a phone may ask the server to run.</summary>
        public static bool Sent(string method) => !Time.Contains(method) && !Queries.Contains(method);
    }
}
