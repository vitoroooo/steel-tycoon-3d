using System;
using System.Linq;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Haul
{
    /// <summary>Work orders from the mill (PRD C.1): a 5% bond, deliveries at the fixed price, +2 on time, bond lost and -5 on failure.</summary>
    public sealed class SpkTests
    {
        private IConfigSource src;
        private LocalGameService g;
        private CityLayout city;

        [SetUp] public void Start()
        {
            src = new OverrideConfigSource(new ResourceConfigSource(), "City", "startCash", 2000000000);
            src = new OverrideConfigSource(src, "Business", "minimumReputation", 1);
            foreach (string slot in new[] { "slot0", "slot1", "slot2" })   // any order may be taken, and one truck load fills the small one
                src = new OverrideConfigSource(src, "Spk", slot + "Reputation", 0);
            src = new OverrideConfigSource(new OverrideConfigSource(src, "Spk", "slot0MinTonnes", 5), "Spk", "slot0MaxTonnes", 5);
            city = new CityLayout(src); g = new LocalGameService(src);
            var p = city.Piles[0]; g.SetPosition(p.Position.X, p.Position.Z);
            g.Classify(p.Id, 0, false); g.PickUp(p.Id);
            g.SetPosition(city.Home.X, city.Home.Z); g.Classify(null, 0, false); g.InspectBid(0); g.SellLocal();
            g.ApplyPermit(true); g.Lease(); g.Advance(24 * 600); g.OpenYard();
            for (int grade = 0; grade < 4; grade++) g.SetBid(grade, 9000);
            g.Advance(96 * 600);   // 250 kg of each grade every fourth hour: six tons of each
        }

        private void Park(SliceLocation post)
        {
            string truck = FleetCatalog.Engkel; var v = g.VehicleOf(truck);
            if (!v.Owned) { g.SetPosition(v.X, v.Z); g.BuyVehicle(truck); }
            g.SetPosition(g.VehicleOf(truck).X, g.VehicleOf(truck).Z); g.EnterVehicle(truck);
            g.ReportVehicle(truck, post.X + 6, post.Z, 0); g.ExitVehicle(post.X + 4, post.Z);
            g.SetPosition(post.X, post.Z);
        }

        private int Grade(string name) => Enumerable.Range(0, 4).First(i => g.YardInfo(i).Grade == name);

        [Test]
        public void TheBoardIsTheSameAllDayAndAnOrderLocksItsBondOnce()
        {
            g.SetPosition(g.GatePost.X, g.GatePost.Z);
            SpkOfferView[] board = g.SpkBoard; Assert.AreEqual(3, board.Length);
            g.Advance(600); Assert.AreEqual(board[1].Tonnes, g.SpkBoard[1].Tonnes); Assert.AreEqual(board[1].PerKg, g.SpkBoard[1].PerKg);
            Assert.AreEqual(board[0].Tonnes * 1000 * board[0].PerKg * .05, board[0].Bond, 1e-6);
            double cash = g.View.Cash; g.TakeSpk(0);
            Assert.AreEqual(cash - board[0].Bond, g.View.Cash, 1e-6); Assert.IsTrue(g.Spk.Active); Assert.IsTrue(g.SpkBoard[0].Taken);
            Assert.Throws<InvalidOperationException>(() => g.TakeSpk(1), "One order at a time");
            g = LocalGameService.Resume(src, g.Save(1), 1, out _);
            Assert.IsTrue(g.Spk.Active); Assert.AreEqual(board[0].PerKg, g.Spk.PerKg);
        }

        [Test]
        public void DeliveringInTimePaysTheOrdersPriceReturnsTheBondAndRaisesReputation()
        {
            g.SetPosition(g.GatePost.X, g.GatePost.Z);
            SpkOfferView order = g.SpkBoard[0]; g.TakeSpk(0); int rep = g.Spk.Reputation;
            Park(g.YardPost); g.LoadTruck(Grade(order.Grade)); g.Advance(600);
            Assert.GreaterOrEqual(g.YardDock.LoadKg, 5000, "The yard holds more than the order asks for");
            Park(g.GatePost);
            HaulView bill = g.GateDock; double cash = g.View.Cash;
            Assert.AreEqual(5000, bill.SpkKg, 1e-6);
            Assert.AreEqual(5000 * order.PerKg + (bill.LoadKg - 5000) * bill.PerKg - bill.RunCost, bill.Net, 1,
                "The order's kilograms at its price, the rest at the day's price");
            g.WeighIn();
            Assert.IsFalse(g.Spk.Active); Assert.AreEqual(cash + order.Bond, g.View.Cash, 1e-6, "The bond comes back at the scale");
            Assert.AreEqual(rep + 2, g.Spk.Reputation); StringAssert.Contains("tepat waktu", g.Spk.Note);
        }

        [Test]
        public void AnOrderLeftUnfilledLosesItsBondAndFiveReputation()
        {
            g.SetPosition(g.GatePost.X, g.GatePost.Z);
            SpkOfferView order = g.SpkBoard[0]; g.TakeSpk(0);
            int rep = g.Spk.Reputation; double cash = g.View.Cash;
            g.Advance((order.Days * 24 + 2) * 600); Assert.IsTrue(g.Spk.Active, "Two hours late still only costs a penalty");
            g.Advance(600);
            Assert.IsFalse(g.Spk.Active); StringAssert.Contains("gagal", g.Spk.Note);
            Assert.AreEqual(Math.Max(0, rep - 5), g.Spk.Reputation); Assert.Greater(rep, 0); Assert.LessOrEqual(g.View.Cash, cash, "The bond is not returned");
        }
    }
}
