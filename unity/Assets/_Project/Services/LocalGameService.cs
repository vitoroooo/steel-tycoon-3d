using System;
using Steel.Core.Economy;
using Steel.Core.Scrap;
using Steel.Core.Time;

namespace Steel.Services
{
    public sealed class LocalGameService : IGameService
    {
        private readonly GameClock clock;
        private readonly AiPriceBook market;
        private readonly ScrapPricing scrap;
        public double GameHours => clock.GameHours;
        public AiQuote Prices => market.Current;

        public LocalGameService(IConfigSource source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var time = new TimeConfig(source.Read("Time"));
            var economy = new EconomyConfig(source.Read("Economy"));
            if (time.RealSecondsPerGameHour != economy.RefreshRealSeconds)
                throw new ArgumentException("Clock tick and price refresh must match.");
            clock = new GameClock(time);
            market = new AiPriceBook(economy);
            scrap = new ScrapPricing(new ScrapConfig(source.Read("Scrap")));
            market.Refresh(0, economy.SupplyTargetKg);
            clock.HourTick += _ => market.Refresh(clock.RealSeconds, economy.SupplyTargetKg);
        }

        public ScrapPile CreatePile(double kilograms, ScrapGrade grade) => new ScrapPile(kilograms, grade);
        public void Advance(double realSeconds) => clock.Advance(realSeconds);
        public double CatchUp(double elapsedRealSeconds) => clock.CatchUp(elapsedRealSeconds);
        public double Quote(ScrapPile pile) => scrap.Quote(pile, Prices.ScrapReference);
    }
}
