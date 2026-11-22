using Steel.Core.Equipment;
using Steel.Core.Fleet;
using Steel.Core.Scrap;
using Steel.Core.State;
using Steel.Core.State.Slice;

namespace Steel.Tests.Maintenance
{
    internal static class MaintenanceFixture
    {
        public static ConfigValues Business => new ConfigValues(TestConfig.Read("Business"));

        public static SliceSession Yard(double cash = 500000000)
        {
            var city = new SliceRules(TestConfig.With("City", "startCash", cash));
            var fleet = new Garage(new MotorRules("triseda", "Triseda", TestConfig.Read("Triseda")));
            var session = new SliceSession(city, new SliceWorld(TestConfig.Read("Piles")),
                new ScrapPricing(new ScrapConfig(TestConfig.Read("Scrap"))), fleet,
                new CartRules(TestConfig.Read("Gerobak")));
            session.Position = city.Home;
            session.ConfigureMaintenance(new MaintenanceRules(TestConfig.Read("Maintenance"), Business));
            session.Yard.applied = session.Yard.leased = session.Yard.bandar = true;
            session.Yard.leaseUntil = 720;
            return session;
        }
    }
}
