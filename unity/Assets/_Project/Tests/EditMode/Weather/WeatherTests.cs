using System;
using NUnit.Framework;
using Steel.Core.State.Saving;
using Steel.Core.Weather;
using Steel.Services;
using UnityEngine;

namespace Steel.Tests.Weather
{
    public sealed class WeatherTests
    {
        private readonly IConfigSource src = new ResourceConfigSource();
        private LocalGameService Loaded(bool roof)
        {
            var g = new LocalGameService(src); var city = new CityLayout(src); var p = city.Piles[0];
            g.SetPosition(p.Position.X,p.Position.Z); g.Inspect(p.Id); g.PickUp(p.Id);
            g.SetPosition(city.Home.X,city.Home.Z); g.SortAtHome(); if (roof) g.Shelter(true); return g;
        }
        [Test] public void ForecastMatchesNextDayAndExposedCargoLosesValueAtFortyEightWetHours()
        {
            var a = Loaded(false); var b = Loaded(true);
            int count = 0, firstDay = 0;
            var forecast = a.Weather(0).Forecast;
            for (int h = 0; count < 48 && h < 1000; h++)
            {
                if (a.RainHere) count++;
                a.Advance(600); b.Advance(600); if (h < 24 && a.RainHere) firstDay++;
                if (h == 0) { Assert.IsTrue(a.RainHere); Assert.IsFalse(a.Weather(2).Rain); }
                if (h == 6) Assert.AreEqual("lumpur",a.Weather(0).Soil);
            }
            StringAssert.StartsWith(firstDay + " dari",forecast);
            Assert.AreEqual(48,count); b.Shelter(false); Assert.AreEqual(.85,a.InspectBid(0).Gross/b.InspectBid(0).Gross,.00001);
            var resumed = LocalGameService.Resume(src,a.Save(1000),1000+86400,out var elapsed);
            Assert.AreEqual(28800,elapsed); a.Advance(elapsed);
            Assert.AreEqual(a.InspectBid(0).Gross,resumed.InspectBid(0).Gross);
        }
        [Test] public void RoofPreservesOnlyTheFirstFiftyTonnes()
        {
            var g = Loaded(false); var s = JsonUtility.FromJson<CareerSave>(g.Save(1000)); var y = s.yard;
            s.cash = 500000000; y.bandar = y.leased = y.applied = true; y.stock[0] = 60000; y.leaseUntil = 1000;
            g = LocalGameService.Resume(src,JsonUtility.ToJson(s),1000,out _); g.ImproveLand(false);
            Assert.AreEqual(380000000,g.View.Cash); Assert.Throws<InvalidOperationException>(() => g.ImproveLand(false));
            int wet = 0; for (int h = 0; wet < 48 && h < 1000; h++) { if (g.RainHere) wet++; g.Advance(600); }
            var saved = JsonUtility.FromJson<CareerSave>(g.Save(2000)).yard;
            Assert.AreEqual(50000 + 10000*.85,saved.BillableKg(0,60000),.001);
        }
        [TestCase(false)] [TestCase(true)]
        public void LoadedTrailerCanBeRescuedAndPaymentArrivesAfterWetUnload(bool tow)
        {
            var g = Loaded(false);
            var s = JsonUtility.FromJson<CareerSave>(g.Save(1000));
            s.cash = 500000000; s.finance.seconds = s.seconds = 7*600; var y = s.yard;
            y.bandar = y.leased = y.applied = true; y.leaseUntil = 1000; y.stock[1] = 15000; y.entries = 3;
            g = LocalGameService.Resume(src,JsonUtility.ToJson(s),1000,out _);
            Assert.Throws<InvalidOperationException>(() => g.ShipGrade(1)); Assert.IsTrue(g.Weather(0).Stuck);
            Assert.AreEqual(15000,g.YardInfo(1).Stock); var before = g.View.Cash;
            g.Rescue(tow); if (!tow) { Assert.Throws<InvalidOperationException>(() => g.ShipGrade(1)); g.Advance(600); }
            double net = g.ShipGrade(1), start = g.View.Cash;
            Assert.AreEqual(before - (tow ? 2500000 : 0),start); Assert.Zero(g.YardInfo(1).Stock);
            g.Advance(600*1.5); Assert.AreEqual(start,g.View.Cash);
            g.Advance(600*.45+.001); Assert.AreEqual(start + net,g.View.Cash,.001);
        }
    }
}
