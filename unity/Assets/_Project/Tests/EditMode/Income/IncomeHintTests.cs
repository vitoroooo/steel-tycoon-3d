using NUnit.Framework;
using Steel.Presentation.UI;
using Steel.Services;

namespace Steel.Tests.Income
{
    /// <summary>The wording a player reads for every state of the street, the cart round and the lapak depot.</summary>
    public sealed class IncomeHintTests
    {
        private readonly CityLayout map = new CityLayout(new ResourceConfigSource());
        private GameView view;
        [SetUp] public void AtTheLapak() =>
            view = new GameView { PlayerX = map.Home.X, PlayerZ = map.Home.Z - 12, CapacityKg = 15 };
        private string Text(IncomeView income, bool brief = false) => IncomeHint.Text(income, view, map, brief);

        [Test] public void BareStreetNamesTheFreeRoundAndTheNextRefill()
        {
            var income = new IncomeView { RestockMinutes = 6.2, RouteKg = 20, RouteMinutes = 30 };
            Assert.AreEqual("Gang kosong. Lapak 12 m U · BUKA ARMADA, pesan rute gratis 20 kg (30 menit).\n" +
                "Tumpukan baru muncul 7 menit lagi.", Text(income));
            Assert.AreEqual("Gang kosong. Lapak 12 m U · BUKA ARMADA, pesan rute gratis 20 kg (30 menit).",
                Text(income, true));
            income.ArrivalMinutes = 29.5;
            Assert.AreEqual("Gang kosong. Kiriman gerobak sampai di lapak 30 menit lagi.\n" +
                "Tumpukan baru muncul 7 menit lagi.", Text(income));
            income.ArrivalMinutes = 0; income.RouteKg = 0;
            Assert.AreEqual("Gang kosong. Tumpukan baru muncul 7 menit lagi.", Text(income, true));
        }

        [Test] public void WaitingDeliveryComesBeforeTheStreet()
        {
            var income = new IncomeView { DepotKg = 20, StreetSites = 4, SiteKg = 12 };
            Assert.AreEqual("Lapak 12 m U · Kiriman 20 kg sudah datang. BUKA ARMADA, AMBIL KIRIMAN.", Text(income));
        }

        [Test] public void StreetSiteSaysWhetherItFitsAndWhenTheCartIsBack()
        {
            var income = new IncomeView { StreetSites = 2, SiteX = map.Home.X + 30, SiteZ = map.Home.Z - 12, SiteKg = 12 };
            Assert.AreEqual("Gang 30 m T · Periksa dulu, baru pungut. Sisa 2 tumpukan.", Text(income));
            income.SiteKg = 18;
            Assert.AreEqual("Gang 30 m T · Tumpukan 18 kg, cuma muat 15 kg. Ambil semuatnya, sisanya aman.",
                Text(income));
            income.CartAwayMinutes = 11.4;
            Assert.AreEqual("Gang 30 m T · Tumpukan 18 kg, cuma muat 15 kg. Ambil dulu, gerobak pulang 12 menit lagi.",
                Text(income));
            view.CapacityKg = 50;
            Assert.AreEqual("Gang 30 m T · Periksa dulu, baru pungut. Sisa 2 tumpukan.", Text(income));
        }

        [Test] public void LetterAnswersWhereMoneyComesFromOnABareStreet()
        {
            var income = new IncomeView { RestockEveryMinutes = 10, RouteKg = 20, RouteMinutes = 30 };
            Assert.AreEqual("Pak Darto  ·  Gang kosong?\nTiap 10 menit warga buang rongsok lagi. Daripada " +
                "bengong, ke lapak: BUKA ARMADA, PESAN RUTE. Gerobak pulang 30 menit lagi bawa 20 kg, gratis.  ›",
                IncomeHint.Letter(income));
            Assert.LessOrEqual(IncomeHint.Letter(income).Length, 171, "No longer than the letter it replaces");
            income.RouteKg = 0;
            StringAssert.EndsWith("warga buang rongsok lagi. Lihat petunjuk di bawah layar.  ›", IncomeHint.Letter(income));
        }
    }
}
