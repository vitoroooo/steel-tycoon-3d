using NUnit.Framework;
using Steel.Core.Fleet;
using Steel.Core.Scrap;
using Steel.Core.State;
using Steel.Core.State.Slice;

namespace Steel.Tests.Crew
{
    public abstract class CrewFixture
    {
        protected SliceSession Game;
        protected ConfigValues Business;

        [SetUp]
        public void OpenLapak()
        {
            var rules = new SliceRules(TestConfig.With("City", "startCash", 100000000));
            var vehicles = new Garage(new MotorRules("pikap", "Pikap", TestConfig.Read("Pikap")));
            Game = new SliceSession(rules, new SliceWorld(TestConfig.Read("Piles")),
                new ScrapPricing(new ScrapConfig(TestConfig.Read("Scrap"))), vehicles,
                new CartRules(TestConfig.Read("Gerobak")));
            Game.InitCrew(new ConfigValues(TestConfig.Read("Crew")));
            Game.Position = rules.Home;
            Business = new ConfigValues(TestConfig.Read("Business"));
            Game.ConfigureMaintenance(new Steel.Core.Equipment.MaintenanceRules(TestConfig.Read("Maintenance"),Business));
            Game.Yard.bandar = Game.Yard.applied = Game.Yard.leased = true;
            Game.Yard.leaseUntil = 10000;
            Game.Yard.bids[1] = 1;
            Game.CreditCrewSaleReputation(3000);
        }

        protected void Receive(long hour, double condition = 100) =>
            Game.CrewReceive(1, 1, 100, 60000, hour, condition);
    }
}
