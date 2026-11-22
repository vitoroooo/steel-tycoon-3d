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
            millRules = new Core.State.ConfigValues(source.Read("Mill"));
            queueLimit = time.OfflineCapRealSeconds; hourSeconds = time.RealSecondsPerGameHour;
            routeConfig = new Core.State.ConfigValues(source.Read("Routes"));
            var economy = new EconomyConfig(source.Read("Economy"));
            supplyTargetKg = economy.SupplyTargetKg;
            if (time.RealSecondsPerGameHour != economy.RefreshRealSeconds)
                throw new ArgumentException("Clock tick and price refresh must match.");
            clock = new GameClock(time);
            market = new AiPriceBook(economy);
            scrap = new ScrapPricing(new ScrapConfig(source.Read("Scrap")));
            CreateSession(source);
            InitMaintenance(source); InitCrew(source);
            session.CrewExpense = session.ChargeOperatingExpense;
            session.MaintenanceAccident += hour => session.InvestigateCrew(hour, session.Caste);
            session.MaintenanceOffline = () => session.CrewOffline;
            session.RepairPaid += cost => session.RecordOperatingProfit(-cost);
            market.Refresh(0, economy.SupplyTargetKg);
            clock.HourTick += hour => market.RefreshBucket(hour, economy.SupplyTargetKg);
            clock.HourTick += session.Routes.OnHour;
            clock.HourTick += _ => FinanceTime();
            clock.HourTick += hour => session.CrewHour(hour, 0, session.Mill.shredder && session.Mill.pendingKg > 0 &&
                hour - 1 < session.Mill.completedAt, session.Mill.opened, session.Mill.rfid);
            clock.HourTick += session.WeatherHour;
            clock.HourTick += SupplierHour;
            clock.HourTick += TenderHour;
            clock.HourTick += SiteHour;
            clock.HourTick += hour => session.AdvanceRisk(hour);
            clock.HourTick += EventHour;
        }

        public ScrapPile CreatePile(double kilograms, ScrapGrade grade) => new ScrapPile(kilograms, grade);
        public void Advance(double realSeconds)
        { clock.Advance(realSeconds); FinishTime(realSeconds); }
        private void FinishTime(double elapsed)
        {
            session.Fleet.AdvanceTrip(elapsed); session.Routes.Advance(elapsed); session.World.Advance(GameHours);
            AdvanceTrade();
            FinanceTime();
            session.PayShipment(GameHours); session.FinishHeat(GameHours);
        }
        public double CatchUp(double elapsedRealSeconds)
        {
            double applied = session.CrewCatchUp(elapsedRealSeconds,queueLimit,Advance);
            FinanceTime(elapsedRealSeconds-applied); return applied;
        }
        public double Quote(ScrapPile pile) => scrap.Quote(pile, Prices.ScrapReference);
    }
}
