using System;
using NUnit.Framework;
using Steel.Core.Fleet;
using Steel.Core.Scrap;
using Steel.Core.State.Slice;
using Steel.Services;
using Steel.Tests.Support;

namespace Steel.Tests.Fleet
{
    public sealed class MotorTests
    {
        private const string Id = FleetCatalog.Pikap;
        private MotorState car;
        private static VehicleView Car(IGameService game) => game.VehicleOf(Id);
        [SetUp] public void Setup()
        {
            car = new MotorState(new MotorRules("pikap", "pikap", new ResourceConfigSource().Read("Pikap")));
            car.Buy();
        }
        [Test] public void PrdCapacityAndTripAreConfigured()
        {
            Assert.AreEqual(1000, car.Cargo.CapacityKg); Assert.AreEqual(3600, car.Rules.TripRealSeconds);
            Assert.AreEqual(85000000, car.Rules.Price);
            var pile = new ScrapPile(1001, ScrapGrade.Hms1); pile.Inspect();
            Assert.Throws<InvalidOperationException>(() => car.Cargo.Add(pile)); Assert.AreEqual(0, car.Cargo.CarriedKg);
        }
        [Test] public void TripPausesOutsideVehicleAndNeverRunsBelowZero()
        {
            car.AdvanceTrip(100); Assert.AreEqual(3600, car.RemainingTripSeconds);
            car.Enter(car.Position); car.AdvanceTrip(3599); Assert.AreEqual(1, car.RemainingTripSeconds);
            car.AdvanceTrip(2); Assert.AreEqual(0, car.RemainingTripSeconds);
            car.Exit(car.Position); car.AdvanceTrip(100); Assert.AreEqual(0, car.RemainingTripSeconds);
            car.Enter(car.Position); Assert.AreEqual(3600, car.RemainingTripSeconds);
        }
        [Test] public void EnterAndExitValidateDistanceAndSpeed()
        {
            Assert.Throws<InvalidOperationException>(() => car.Enter(new WorldPoint(100, 100)));
            car.Enter(car.Position);
            Assert.Throws<InvalidOperationException>(() => car.Enter(car.Position));
            car.ReportPose(car.Position, 1);
            Assert.Throws<InvalidOperationException>(() => car.Exit(car.Position));
            car.ReportPose(car.Position, 0);
            Assert.Throws<InvalidOperationException>(() => car.Exit(new WorldPoint(100, 100)));
            car.Exit(car.Position); Assert.IsFalse(car.Occupied);
        }
        [TestCase(double.NaN)] [TestCase(-1)] [TestCase(double.PositiveInfinity)]
        public void InvalidElapsedTimeIsRejected(double seconds) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => car.AdvanceTrip(seconds));
        [Test] public void OverspeedAndUnoccupiedDrivingAreRejected()
        {
            Assert.Throws<InvalidOperationException>(() => car.ReportPose(car.Position, 1));
            car.Enter(car.Position);
            Assert.Throws<ArgumentOutOfRangeException>(() => car.ReportPose(car.Position, 13));
            Assert.Throws<ArgumentOutOfRangeException>(() => car.ReportPose(car.Position, -5));
            Assert.Throws<ArgumentOutOfRangeException>(() => car.ReportPose(car.Position, double.NaN));
        }
        [Test] public void PickupCargoCanExceedBackpackAndSellThroughService()
        {
            IGameService game = new LocalGameService(Wealthy.Source());
            var layout = new CityLayout(new ResourceConfigSource());
            var first = layout.Piles[0]; game.SetPosition(first.Position.X, first.Position.Z);
            game.Inspect(first.Id); game.PickUp(first.Id);
            Wealthy.Own(game, Id); game.EnterVehicle(Id);
            Assert.AreEqual(12, Car(game).CargoKg); Assert.AreEqual(1000, game.View.CapacityKg);
            for (int i = 1; i < layout.Piles.Length; i++)
            {
                var pile = layout.Piles[i]; game.ReportVehicle(Id, pile.Position.X, pile.Position.Z, 0);
                game.Inspect(pile.Id); game.PickUp(pile.Id);
            }
            Assert.Greater(Car(game).CargoKg, 50);
            game.ReportVehicle(Id, layout.Home.X, layout.Home.Z, 0); game.SortAtHome();
            game.ReportVehicle(Id, layout.Factory.X, layout.Factory.Z, 0);
            double cash = game.View.Cash; game.PreviewSale(); game.ConfirmSale();
            Assert.Greater(game.View.Cash, cash); Assert.AreEqual(24, Car(game).CargoKg);
            game.ReportVehicle(Id, layout.Home.X, layout.Home.Z, 0);
            var reuse = game.InspectBid(2);
            Assert.Greater(reuse.Freight, 0); Assert.AreEqual(reuse.Net, game.SellLocal());
            Assert.AreEqual(0, Car(game).CargoKg);
            game.ExitVehicle(Car(game).X + 2, Car(game).Z); Assert.AreEqual(15, game.View.CapacityKg);
        }
    }
}
