using NUnit.Framework;
using Steel.Presentation.UI;
using Steel.Services;

namespace Steel.Tests
{
    /// <summary>The beginner's reference quotes the running game, so its figures cannot drift from the rules.</summary>
    public sealed class GuideBookTests
    {
        private LocalGameService g;
        [SetUp] public void Start() => g = new LocalGameService(new ResourceConfigSource());

        [Test] public void MoneyTopicNamesRefillAndTheFreeCartRoundWithLiveFigures()
        {
            Assert.AreEqual("1/5  ·  Duit dari mana?", GuideBook.Heading(0));
            string page = GuideBook.Page(0, g);
            StringAssert.Contains("tiap 5 menit warga buang rongsok lagi", page);
            StringAssert.Contains("BUKA ARMADA, lalu PESAN RUTE", page);
            StringAssert.Contains("30 menit lagi dia pulang bawa 20 kg", page);
        }

        [Test] public void LoadAndFleetTopicsQuoteCartAndMotorFigures()
        {
            StringAssert.Contains("Dorong gerobak, muat 50 kg", GuideBook.Page(1, g));
            StringAssert.Contains("Ambil semuatnya dulu", GuideBook.Page(1, g));
            StringAssert.Contains("Rp12.000.000", GuideBook.Page(3, g));
            StringAssert.Contains("sekali jalan 45 menit, pulang bawa 120 kg", GuideBook.Page(3, g));
        }

        [Test] public void IdlePromptPointsToTheGuideOnlyUntilItHasBeenOpened()
        {
            StringAssert.EndsWith("\nBingung? Buku usaha › PANDUAN", GuideBook.Idle(false));
            Assert.AreEqual("Geser kiri untuk bergerak · Usap kanan untuk melihat", GuideBook.Idle(true));
            foreach (string line in GuideBook.Idle(false).Split('\n')) Assert.LessOrEqual(line.Length, 44, line);
        }

        [Test] public void OpeningTheNotesIsRememberedAcrossResume()
        {
            Assert.IsFalse(g.GuideRead);
            g.ReadGuide();
            var resumed = LocalGameService.Resume(new ResourceConfigSource(), g.Save(5000), 5000, out _);
            Assert.IsTrue(resumed.GuideRead);
        }

        [Test] public void EveryTopicFitsTheFourLinesOfTheBookPage()
        {
            for (int topic = 0; topic < GuideBook.Topics.Length; topic++)
            {
                string page = GuideBook.Page(topic, g);
                Assert.That(page.Length, Is.InRange(120, 240), GuideBook.Topics[topic]);
                StringAssert.DoesNotContain("\n", page);
            }
        }
    }
}
