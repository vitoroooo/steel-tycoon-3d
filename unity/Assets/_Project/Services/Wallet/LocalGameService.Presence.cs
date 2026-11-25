namespace Steel.Services
{
    /// <summary>What the server shows other players of this one.</summary>
    public sealed partial class LocalGameService
    {
        public float PresenceX => (float)session.Position.X;
        public float PresenceZ => (float)session.Position.Z;
        public string PresenceVehicle => session.Fleet.Driven?.Rules.Id ?? "";
        /// <summary>Where the dealer parks a vehicle, and where one that fell off the map is put back.</summary>
        public SliceLocation VehicleHome(string id) => new SliceLocation(session.Fleet.Find(id).Rules.Spawn);
    }
}
