namespace Steel.Services
{
    /// <summary>What a scavenger can earn from right now: street sites, the free cart round and waiting deliveries.</summary>
    public sealed class IncomeView
    {
        /// <summary>Street sites that still hold scrap, and the best one to walk to (nearest that fits the load).</summary>
        public int StreetSites;
        public float SiteX, SiteZ;
        public double SiteKg;
        /// <summary>Real minutes until residents refill the next emptied site; zero while none is empty.</summary>
        public double RestockMinutes;
        /// <summary>Scrap already delivered to the lapak and waiting to be taken.</summary>
        public double DepotKg;
        /// <summary>Real minutes until the next round under way arrives; zero when nothing is under way.</summary>
        public double ArrivalMinutes;
        /// <summary>The free cart round on offer; zero kilograms once the district is picked clean for the day.</summary>
        public double RouteKg, RouteMinutes;
    }

    public partial interface IGameService
    {
        IncomeView Income { get; }
        /// <summary>True once Pak Darto has explained where money comes from when the street is bare.</summary>
        bool StreetTipRead { get; }
        void ReadStreetTip();
    }
}
