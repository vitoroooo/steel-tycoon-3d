using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Launch
{
    public sealed class MenuTextTests
    {
        private static readonly CareerSummary Saved = CareerSummary.Saved(1250000.9, 3, 2, 10000);

        [Test] public void MoneyUsesDotsOnEveryPhoneLanguage()
        {
            Assert.AreEqual("Rp 0", MenuText.Money(0));
            Assert.AreEqual("Rp 999", MenuText.Money(999.99));
            Assert.AreEqual("Rp 1.250.000", MenuText.Money(1250000.9));
            Assert.AreEqual("Rp 200.000.000", MenuText.Money(200000000));
            Assert.AreEqual("-Rp 1.500", MenuText.Money(-1500));
        }

        [Test] public void MainButtonSaysWhatItWillDo()
        {
            Assert.AreEqual("LANJUTKAN", MenuText.Play(CareerState.Saved));
            Assert.AreEqual("MULAI KARIER", MenuText.Play(CareerState.Empty));
            Assert.AreEqual("MAIN TANPA SIMPAN", MenuText.Play(CareerState.Unreadable));
        }

        [Test] public void CardSummarisesTheSavedCareer()
        {
            Assert.AreEqual("KARIER TERSIMPAN", MenuText.CardTitle(CareerState.Saved));
            Assert.AreEqual("Hari ke-3 · Kasta 2\nKas Rp 1.250.000\nTerakhir main 2 jam lalu",
                MenuText.CardBody(Saved, 10000 + 2 * 3600 + 59));
            StringAssert.Contains("dari nol", MenuText.CardBody(CareerSummary.Empty, 0));
            StringAssert.Contains("tidak akan menimpanya", MenuText.CardBody(CareerSummary.Unreadable, 0));
        }

        [Test] public void LastPlayedReadsNaturallyAndSurvivesAClockSetBack()
        {
            Assert.AreEqual("baru saja", MenuText.Ago(-500));
            Assert.AreEqual("baru saja", MenuText.Ago(59));
            Assert.AreEqual("1 menit lalu", MenuText.Ago(60));
            Assert.AreEqual("59 menit lalu", MenuText.Ago(3599));
            Assert.AreEqual("23 jam lalu", MenuText.Ago(86399));
            Assert.AreEqual("3 hari lalu", MenuText.Ago(3 * 86400 + 5));
        }

        [Test] public void ConfirmNamesWhatIsReplaced()
        {
            string saved = MenuText.Confirm(Saved);
            StringAssert.Contains("Hari ke-3", saved); StringAssert.Contains("Rp 1.250.000", saved);
            StringAssert.Contains("tidak bisa dilanjutkan", saved);
            StringAssert.Contains("tidak terbaca", MenuText.Confirm(CareerSummary.Unreadable));
        }

        [Test] public void LoadingAndGraphicsCaptions()
        {
            Assert.AreEqual("Memuat Sukabaja… 0%", MenuText.Loading(-1));
            Assert.AreEqual("Memuat Sukabaja… 50%", MenuText.Loading(.5f));
            Assert.AreEqual("Memuat Sukabaja… 100%", MenuText.Loading(7));
            Assert.AreEqual("Grafis: Ringan  ›", MenuText.Graphics("Ringan"));
        }
    }
}
