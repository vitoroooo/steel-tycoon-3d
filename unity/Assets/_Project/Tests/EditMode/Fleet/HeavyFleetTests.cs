using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Fleet
{
    /// <summary>PRD appendix A: the expedition trucks of the upper tiers, and careers saved before they existed.</summary>
    public sealed class HeavyFleetTests
    {
        private static IGameService Rich(double cash) =>
            new LocalGameService(new OverrideConfigSource(new ResourceConfigSource(), "City", "startCash", cash));

        [Test] public void EngkelAndTrailerMatchThePrdTable()
        {
            var game = new LocalGameService(new ResourceConfigSource());
            Assert.AreEqual(8000, game.VehicleOf(FleetCatalog.Engkel).CapacityKg);
            Assert.AreEqual(320000000, game.VehicleOf(FleetCatalog.Engkel).Price);
            Assert.AreEqual(20000, game.VehicleOf(FleetCatalog.Trailer).CapacityKg);
            Assert.AreEqual(900000000, game.VehicleOf(FleetCatalog.Trailer).Price);
        }

        [Test] public void EveryHeavierTruckCarriesMoreAndCostsMore()
        {
            var game = new LocalGameService(new ResourceConfigSource());
            VehicleView lighter = game.VehicleOf(FleetCatalog.Pikap);
            foreach (string id in FleetCatalog.Heavy)
            {
                VehicleView truck = game.VehicleOf(id);
                Assert.Greater(truck.CapacityKg, lighter.CapacityKg, id);
                Assert.Greater(truck.Price, lighter.Price, id);
                Assert.Less(truck.MaxSpeed, lighter.MaxSpeed, id + " must be slower than what it replaces");
                lighter = truck;
            }
        }

        [Test] public void ATruckIsBoughtOnceAndOnlyWithEnoughCash()
        {
            var poor = Rich(319999999);
            poor.SetPosition(poor.VehicleOf(FleetCatalog.Engkel).X, poor.VehicleOf(FleetCatalog.Engkel).Z);
            Assert.Throws<InvalidOperationException>(() => poor.BuyVehicle(FleetCatalog.Engkel));
            var game = Rich(320000000);
            game.SetPosition(game.VehicleOf(FleetCatalog.Engkel).X, game.VehicleOf(FleetCatalog.Engkel).Z);
            game.BuyVehicle(FleetCatalog.Engkel);
            Assert.AreEqual(0, game.View.Cash);
            Assert.IsTrue(game.VehicleOf(FleetCatalog.Engkel).Owned);
            Assert.Throws<InvalidOperationException>(() => game.BuyVehicle(FleetCatalog.Engkel));
        }

        [Test] public void CareerSavedBeforeTheTrucksExistedStillLoads()
        {
            var source = new OverrideConfigSource(new ResourceConfigSource(), "City", "startCash", 100000000d);
            var game = new LocalGameService(source);
            game.SetPosition(game.VehicleOf(FleetCatalog.Pikap).X, game.VehicleOf(FleetCatalog.Pikap).Z);
            game.BuyVehicle(FleetCatalog.Pikap);
            string old = game.Save(1000);
            foreach (string id in FleetCatalog.Heavy)   // drop the trucks, as a save from the previous version has none
                old = Regex.Replace(old, ",?\\{\"id\":\"" + id + "\".*?\"cargo\":\\[\\]\\}", "");
            StringAssert.DoesNotContain(FleetCatalog.Tronton, old);
            var resumed = LocalGameService.Resume(source, old, 1000, out _);
            Assert.IsTrue(resumed.VehicleOf(FleetCatalog.Pikap).Owned);
            Assert.IsFalse(resumed.VehicleOf(FleetCatalog.Tronton).Owned);
            Assert.AreEqual(15000000, resumed.View.Cash);
        }

        [Test] public void SaveNamingAnUnknownVehicleIsRejected()
        {
            var source = new ResourceConfigSource();
            string tampered = new LocalGameService(source).Save(1000).Replace("\"id\":\"tronton\"", "\"id\":\"kapal\"");
            Assert.Throws<FormatException>(() => LocalGameService.Resume(source, tampered, 1000, out _));
        }
    }
}
