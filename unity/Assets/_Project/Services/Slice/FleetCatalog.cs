using Steel.Core.Fleet;

namespace Steel.Services
{
    /// <summary>The drivable vehicles of PRD 5.1 and the config file each one is tuned in.</summary>
    public static class FleetCatalog
    {
        public const string Triseda = "triseda";
        public const string Pikap = "pikap";

        public static Garage Create(IConfigSource source) => new Garage(
            new MotorRules(Triseda, "triseda", source.Read("Triseda")),
            new MotorRules(Pikap, "pikap", source.Read("Pikap")));
    }
}
