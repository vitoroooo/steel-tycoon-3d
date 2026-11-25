using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.Services;
using Steel.PlayTests.Support;
using Steel.Presentation.Input;
using Steel.Presentation.UI;
using Steel.Presentation.Vehicles;

namespace Steel.PlayTests
{
    public sealed class VehicleSaleTests
    {
        private sealed class Throttle : IMoveInput
        {
            public Vector2 Value;
            public Vector2 Move => Value;
        }
        [UnityTest] public IEnumerator OpeningOfferWhileMovingStopsPickupAndSaleSucceeds()
        {
            yield return OwnedFleet.LoadWealthyCity();
            var host = Object.FindObjectOfType<GameHost>(); var game = host.Game; var layout = host.Layout;
            var driver = OwnedFleet.Buy(host, FleetCatalog.Pikap); var body = driver.GetComponent<Rigidbody>();
            var hud = Object.FindObjectOfType<CityHud>(); var input = Object.FindObjectOfType<SliceInput>();
            var pile = layout.Piles[0].Position;
            game.SetPosition(pile.X, pile.Z); game.Inspect("p0"); game.PickUp("p0");
            game.SetPosition(layout.Home.X, layout.Home.Z); game.SortAtHome();
            game.SetPosition(game.VehicleOf(FleetCatalog.Pikap).X, game.VehicleOf(FleetCatalog.Pikap).Z); driver.Toggle();
            var throttle = new Throttle(); driver.MoveInput = throttle;
            body.position = new Vector3(layout.Factory.X - 2.5f, .035f, layout.Factory.Z - 1);
            body.rotation = Quaternion.Euler(0, 90, 0);
            game.ReportVehicle(FleetCatalog.Pikap, body.position.x, body.position.z, 0);
            float end = Time.fixedTime + .12f;
            while (Time.fixedTime < end) yield return new WaitForFixedUpdate();
            throttle.Value = Vector2.up; end = Time.fixedTime + .6f;
            while (Time.fixedTime < end) yield return new WaitForFixedUpdate();
            Assert.Greater(Mathf.Abs(game.VehicleOf(FleetCatalog.Pikap).Speed), 2);
            Assert.AreEqual("factory", game.Nearby.Id); hud.Act();
            Assert.IsTrue(hud.SalePanel.activeSelf); Assert.IsTrue(input.IsBlocked);
            Vector3 atOffer = body.position; double cash = game.View.Cash;
            end = Time.fixedTime + 2;
            while (Time.fixedTime < end) yield return new WaitForFixedUpdate();
            Assert.Less(Vector3.Distance(atOffer, body.position), .01f);
            Assert.AreEqual(0, game.VehicleOf(FleetCatalog.Pikap).Speed);
            hud.Confirm();
            Assert.IsFalse(hud.SalePanel.activeSelf); Assert.IsFalse(input.IsBlocked);
            Assert.Greater(game.View.Cash, cash); Assert.AreEqual(0, game.View.CarriedKg);
        }
    }
}
