using System;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Fleet
{
    public sealed class CartTests
    {
        private LocalGameService game;
        private CityLayout layout;

        [SetUp] public void SetUp()
        {
            var source = new ResourceConfigSource();
            game = new LocalGameService(source);
            layout = new CityLayout(source);
        }

        private void StandAtCart() => game.SetPosition(game.Cart.X, game.Cart.Z);

        [Test] public void CartStartsParkedWithPrdCapacity()
        {
            Assert.IsFalse(game.Cart.Held);
            Assert.AreEqual(50, game.Cart.CapacityKg);
        }

        [Test] public void GrabbingFromFarAwayIsRefused()
        {
            game.SetPosition(game.Cart.X + 30, game.Cart.Z);
            Assert.Throws<InvalidOperationException>(() => game.GrabCart());
        }

        [Test] public void HeldCartTravelsWithPlayerAndParkedCartStays()
        {
            StandAtCart();
            game.GrabCart();
            game.SetPosition(5, 7);
            Assert.AreEqual(5, game.Cart.X); Assert.AreEqual(7, game.Cart.Z);
            game.ReleaseCart();
            game.SetPosition(40, 40);
            Assert.AreEqual(5, game.Cart.X); Assert.AreEqual(7, game.Cart.Z);
        }

        [Test] public void HandLoadMovesIntoCartAndStaysThereAfterRelease()
        {
            var pile = layout.Piles[0];
            game.SetPosition(pile.Position.X, pile.Position.Z);
            game.Inspect(pile.Id); game.PickUp(pile.Id);
            double carried = game.View.CarriedKg;
            StandAtCart();
            game.GrabCart();
            Assert.AreEqual(carried, game.Cart.CargoKg);
            Assert.AreEqual(50, game.View.CapacityKg);
            game.ReleaseCart();
            Assert.AreEqual(0, game.View.CarriedKg, "Hands must be empty after parking the cart");
            Assert.AreEqual(carried, game.Cart.CargoKg);
        }

        [Test] public void PickupCannotBeEnteredWhilePushingCart()
        {
            StandAtCart();
            game.GrabCart();
            game.SetPosition(game.Vehicle.X, game.Vehicle.Z);
            Assert.Throws<InvalidOperationException>(() => game.EnterVehicle());
        }
    }
}
