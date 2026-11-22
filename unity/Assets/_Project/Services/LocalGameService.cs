using System;
using Steel.Core.Economy;
using Steel.Core.Scrap;
using Steel.Core.Time;

namespace Steel.Services
{
    public sealed partial class LocalGameService : IGameService
    {
        private readonly GameClock clock;
        private readonly AiPriceBook market;
        private readonly ScrapPricing scrap;
        private readonly double supplyTargetKg;
        public double GameHours => clock.GameHours;
        public AiQuote Prices => market.Current;

        public LocalGameService(IConfigSource source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var time = new TimeConfig(source.Read("Time"));
            business = new Core.State.ConfigValues(source.Read("Business"));
            queueLimit = time.OfflineCapRealSeconds;
            routeConfig = new Core.State.ConfigValues(source.Read("Routes"));
            var economy = new EconomyConfig(source.Read("Economy"));
            supplyTargetKg = economy.SupplyTargetKg;
            if (time.RealSecondsPerGameHour != economy.RefreshRealSeconds)
                throw new ArgumentException("Clock tick and price refresh must match.");
            clock = new GameClock(time);
            market = new AiPriceBook(economy);
            scrap = new ScrapPricing(new ScrapConfig(source.Read("Scrap")));
            CreateSession(source);
            market.Refresh(0, economy.SupplyTargetKg);
            clock.HourTick += hour => market.RefreshBucket(hour, economy.SupplyTargetKg);
            clock.HourTick += session.World.OnHour;
            clock.HourTick += session.Routes.OnHour;
            clock.HourTick += session.WeatherHour;
            clock.HourTick += SupplierHour;
        }

        public ScrapPile CreatePile(double kilograms, ScrapGrade grade) => new ScrapPile(kilograms, grade);
        public void Advance(double realSeconds)
        { clock.Advance(realSeconds); FinishTime(realSeconds); }
        private void FinishTime(double elapsed)
        { session.Fleet.AdvanceTrip(elapsed); session.Routes.Advance(elapsed); session.PayShipment(GameHours); }
        public double CatchUp(double elapsedRealSeconds)
        {
            double applied = clock.CatchUp(elapsedRealSeconds);
            FinishTime(applied); return applied;
        }
        public double Quote(ScrapPile pile) => scrap.Quote(pile, Prices.ScrapReference);
    }
}
