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
            Assert.AreEqual("1/5  ·  Dari mana uang?", GuideBook.Heading(0));
            string page = GuideBook.Page(0, g);
            StringAssert.Contains("satu tumpukan tiap 5 menit", page);
            StringAssert.Contains("BUKA ARMADA lalu PESAN RUTE", page);
            StringAssert.Contains("20 kg gratis dalam 30 menit", page);
        }

        [Test] public void LoadAndFleetTopicsQuoteCartAndMotorFigures()
        {
            StringAssert.Contains("gerobak memuat 50 kg", GuideBook.Page(1, g));
            StringAssert.Contains("dipungut sebagian", GuideBook.Page(1, g));
            StringAssert.Contains("Rp12.000.000", GuideBook.Page(3, g));
            StringAssert.Contains("120 kg tiap 45 menit", GuideBook.Page(3, g));
        }

        [Test] public void IdlePromptPointsToTheGuideOnlyUntilItHasBeenOpened()
        {
            StringAssert.EndsWith("\nPanduan: Buku usaha › PANDUAN", GuideBook.Idle(false));
            Assert.AreEqual("Geser kiri untuk bergerak · Usap kanan untuk melihat", GuideBook.Idle(true));
            foreach (string line in GuideBook.Idle(false).Split('\n')) Assert.LessOrEqual(line.Length, 44, line);
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
