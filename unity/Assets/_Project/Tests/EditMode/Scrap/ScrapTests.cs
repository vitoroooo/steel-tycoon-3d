using System;
using NUnit.Framework;
using Steel.Core.Scrap;

namespace Steel.Tests.Scrap
{
    public sealed class ScrapTests
    {
        private ScrapPricing pricing;

        [SetUp]
        public void SetUp() => pricing = new ScrapPricing(new ScrapConfig(TestConfig.Read("Scrap")));

        [Test]
        public void InspectionRevealsGradeButSortingUnlocksGradePrice()
        {
            var pile = new ScrapPile(100, ScrapGrade.Hms1);
            Assert.IsNull(pile.KnownGrade);
            Assert.AreEqual(385000, pricing.Quote(pile, 5500), 1e-8);
            Assert.Throws<InvalidOperationException>(() => pile.Sort());
            Assert.AreEqual(ScrapGrade.Hms1, pile.Inspect());
            Assert.AreEqual(385000, pricing.Quote(pile, 5500), 1e-8);
            pile.Sort();
            Assert.AreEqual(550000, pricing.Quote(pile, 5500), 1e-8);
        }

        [TestCase(ScrapGrade.Hms1, 1)]
        [TestCase(ScrapGrade.Hms2, .85)]
        [TestCase(ScrapGrade.CastIron, .9)]
        [TestCase(ScrapGrade.Mixed, .7)]
        [TestCase(ScrapGrade.Reuse, 1.6)]
        [TestCase(ScrapGrade.Dirt, 0)]
        public void SortedGradesMatchPrdTable(ScrapGrade grade, double factor)
        {
            var pile = new ScrapPile(100, grade);
            pile.Inspect();
            pile.Sort();
            Assert.AreEqual(100 * 5500 * factor, pricing.Quote(pile, 5500), 1e-8);
        }

        [Test]
        public void DirtReducesNetWeightOnlyOnceAndReuseHasConfiguredRange()
        {
            var pile = new ScrapPile(100, ScrapGrade.Hms1, .2);
            pile.Inspect();
            pile.Sort();
            pile.Sort();
            Assert.AreEqual(80, pile.SaleWeightKg);
            Assert.AreEqual(440000, pricing.Quote(pile, 5500), 1e-8);
            var reuse = new ScrapPile(100, ScrapGrade.Reuse, reuseQuality: 1);
            reuse.Inspect();
            reuse.Sort();
            Assert.AreEqual(1375000, pricing.Quote(reuse, 5500), 1e-8);
        }

        [Test]
        public void EmptyPileAndZeroReferenceHaveZeroValue()
        {
            Assert.AreEqual(0, pricing.Quote(new ScrapPile(0, ScrapGrade.Hms1), 5500));
            Assert.AreEqual(0, pricing.Quote(new ScrapPile(100, ScrapGrade.Hms1), 0));
        }

        [Test]
        public void InvalidInputsAreRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ScrapPile(-1, ScrapGrade.Hms1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ScrapPile(1, (ScrapGrade)99));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ScrapPile(1, ScrapGrade.Hms1, 1.1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ScrapPile(1, ScrapGrade.Reuse, reuseQuality: double.NaN));
            Assert.Throws<ArgumentNullException>(() => pricing.Quote(null, 5500));
        }
    }
}
