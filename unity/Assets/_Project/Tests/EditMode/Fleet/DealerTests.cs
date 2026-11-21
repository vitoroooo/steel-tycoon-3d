using System;
using NUnit.Framework;
using Steel.Services;
using Steel.Tests.Support;

namespace Steel.Tests.Fleet
{
    /// <summary>PRD 5.1: the triseda costs Rp12 juta and carries 300 kg; nothing drives until it is paid for.</summary>
    public sealed class DealerTests
    {
        private const string Triseda = FleetCatalog.Triseda, Pikap = FleetCatalog.Pikap;

        private static void StandAt(IGameService game, string id) =>
            game.SetPosition(game.VehicleOf(id).X, game.VehicleOf(id).Z);

        [Test] public void FleetMatchesThePrdTable()
        {
            var game = new LocalGameService(new ResourceConfigSource());
            Assert.AreEqual(300, game.VehicleOf(Triseda).CapacityKg);
            Assert.AreEqual(12000000, game.VehicleOf(Triseda).Price);
            Assert.AreEqual(1000, game.VehicleOf(Pikap).CapacityKg);
            Assert.AreEqual(85000000, game.VehicleOf(Pikap).Price);
        }

        [Test] public void StartingCashCannotBuyOrDriveAnything()
        {
            var game = new LocalGameService(new ResourceConfigSource());
            foreach (string id in new[] { Triseda, Pikap })
            {
                StandAt(game, id);
                Assert.Throws<InvalidOperationException>(() => game.BuyVehicle(id));
                Assert.Throws<InvalidOperationException>(() => game.EnterVehicle(id));
                Assert.IsFalse(game.VehicleOf(id).Owned);
            }
            Assert.AreEqual(500000, game.View.Cash);
        }

        [Test] public void BuyingChargesThePriceOnceAndUnlocksTheSaddle()
        {
            var game = new LocalGameService(Wealthy.Source());
            Assert.IsTrue(Wealthy.Own(game, Triseda).Owned);
            Assert.AreEqual(Wealthy.Cash - 12000000, game.View.Cash);
            Assert.Throws<InvalidOperationException>(() => game.BuyVehicle(Triseda));
            Assert.AreEqual(Wealthy.Cash - 12000000, game.View.Cash);
            game.EnterVehicle(Triseda);
            Assert.IsTrue(game.VehicleOf(Triseda).Occupied);
            Assert.AreEqual(300, game.View.CapacityKg);
            Assert.AreEqual(Triseda, game.Vehicle.Id, "The driven vehicle is the one in focus");
        }

        [Test] public void BuyingNeedsThePlayerAtTheVehicle()
        {
            var game = new LocalGameService(Wealthy.Source());
            game.SetPosition(game.VehicleOf(Triseda).X + 40, game.VehicleOf(Triseda).Z);
            Assert.Throws<InvalidOperationException>(() => game.BuyVehicle(Triseda));
            Assert.AreEqual(Wealthy.Cash, game.View.Cash);
        }

        [Test] public void OnlyOneVehicleIsDrivenAtATime()
        {
            var game = new LocalGameService(Wealthy.Source());
            Wealthy.Own(game, Pikap); Wealthy.Own(game, Triseda);
            game.EnterVehicle(Triseda);
            game.ReportVehicle(Triseda, game.VehicleOf(Pikap).X, game.VehicleOf(Pikap).Z, 0);
            Assert.Throws<InvalidOperationException>(() => game.EnterVehicle(Pikap));
        }
    }
}
