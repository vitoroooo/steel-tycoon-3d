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
            CreateSession(source);
            market.Refresh(0, economy.SupplyTargetKg);
            clock.HourTick += hour => market.RefreshBucket(hour, economy.SupplyTargetKg);
        }

        public ScrapPile CreatePile(double kilograms, ScrapGrade grade) => new ScrapPile(kilograms, grade);
        public void Advance(double realSeconds)
        { clock.Advance(realSeconds); session.Fleet.AdvanceTrip(realSeconds); }
        public double CatchUp(double elapsedRealSeconds)
        {
            double applied = clock.CatchUp(elapsedRealSeconds);
            session.Fleet.AdvanceTrip(applied); return applied;
        }
        public double Quote(ScrapPile pile) => scrap.Quote(pile, Prices.ScrapReference);
    }
}
