using System;
using System.IO;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Services
{
    public sealed class CareerTests
    {
        private readonly ResourceConfigSource source = new ResourceConfigSource();
        private CityLayout layout;
        private LocalGameService g;
        private string folder;
        [SetUp] public void Start()
        {
            folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            g = new LocalGameService(source); layout = new CityLayout(source);
        }
        [TearDown] public void Clean() { if (Directory.Exists(folder)) Directory.Delete(folder, true); }
        private void Pile()
        {
            var p = layout.Piles[0]; g.SetPosition(p.Position.X, p.Position.Z); g.Inspect(p.Id); g.PickUp(p.Id);
        }
        private void Home() => g.SetPosition(layout.Home.X, layout.Home.Z);
        private void Resume(long elapsed)
        {
            string data = g.Save(2000);
            g = LocalGameService.Resume(source, data, 2000 + elapsed, out _);
        }

        [Test] public void CampaignResumesWithSameCargoPurchaseAndGrantCannotRepeat()
        {
            Pile(); Home(); g.SortAtHome();
            Resume(2);
            Assert.IsTrue(g.IsCollected("p0")); Assert.IsTrue(g.View.LoadSorted);
            g.SetPosition(layout.Factory.X, layout.Factory.Z); g.PreviewSale(); g.ConfirmSale();
            var v = g.VehicleOf(FleetCatalog.Triseda); g.SetPosition(v.X, v.Z);
            g.ClaimDealerGrant(); g.BuyVehicle(v.Id); g.EnterVehicle(v.Id);
            double cash = g.View.Cash; Resume(20000);
            Assert.AreEqual(cash, g.View.Cash); Assert.IsTrue(g.VehicleOf(v.Id).Occupied);
            Assert.IsTrue(g.VehicleOf(v.Id).Owned); Assert.IsFalse(g.IsCollected("p0"));
            Assert.Throws<InvalidOperationException>(() => g.ClaimDealerGrant());
        }

        [Test] public void OfflineQueueIsCappedAndClaimCannotReplayAcrossDiskLoads()
        {
            Home(); g.BookRoute(g.Route("cart", 0, false));
            var first = new CareerFiles(folder); first.Write(g, 1000);
            var second = new CareerFiles(folder); g = second.Open(source, 100000, out var elapsed);
            Assert.AreEqual(28800, elapsed); Assert.AreEqual(20, g.Route("cart", 0, false).DepotKg);
            Assert.AreEqual(15, g.CollectRoute()); second.Write(g, 100000);
            g = new CareerFiles(folder).Open(source, 100000, out elapsed);
            Assert.AreEqual(0, elapsed); Assert.AreEqual(15, g.View.CarriedKg);
            Assert.AreEqual(5, g.Route("cart", 0, false).DepotKg);
        }

        [Test] public void InterruptedNewSlotFallsBackAndUnrecoverableCareerIsPreserved()
        {
            Home(); var disk = new CareerFiles(folder); disk.Write(g, 1000);
            Pile(); disk.Write(g, 1001);
            File.WriteAllText(Path.Combine(folder, "sukabaja-b.json"), "{}");
            var next = new CareerFiles(folder);
            g = next.Open(source, 900, out var elapsed);
            Assert.Zero(elapsed); Assert.IsFalse(g.IsCollected("p0")); Assert.IsTrue(next.HasCareer);
            File.WriteAllText(Path.Combine(folder, "sukabaja-a.json"), "{}");
            var bad = new CareerFiles(folder); g = bad.Open(source, 1002, out _);
            Assert.IsTrue(bad.ReadOnly); bad.Write(g, 1100);
            Assert.AreEqual("{}", File.ReadAllText(Path.Combine(folder, "sukabaja-a.json")));
        }
    }
}
