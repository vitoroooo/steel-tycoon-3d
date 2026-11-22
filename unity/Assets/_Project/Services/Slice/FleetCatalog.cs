using Steel.Core.Fleet;

namespace Steel.Services
{
    /// <summary>The drivable vehicles of PRD 5.1 and the config file each one is tuned in.</summary>
    public static class FleetCatalog
    {
        public const string Triseda = "triseda";
        public const string Pikap = "pikap";
        public const string Engkel = "engkel";
        public const string Tronton = "tronton";
        public const string Trailer = "trailer";
        /// <summary>The expedition trucks of the upper tiers, lightest first.</summary>
        public static readonly string[] Heavy = { Engkel, Tronton, Trailer };

        public static Garage Create(IConfigSource source) => new Garage(
            new MotorRules(Triseda, "triseda", source.Read("Triseda")),
            new MotorRules(Pikap, "pikap", source.Read("Pikap")),
            new MotorRules(Engkel, "truk engkel", source.Read("Engkel")),
            new MotorRules(Tronton, "tronton", source.Read("Tronton")),
            new MotorRules(Trailer, "trailer", source.Read("Trailer")));
    }
}
