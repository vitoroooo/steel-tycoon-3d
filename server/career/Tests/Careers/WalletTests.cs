using Steel.Career.Tests.Support;
using Xunit;

namespace Steel.Career.Tests.Careers
{
    /// <summary>One purse (PRD 12): the cash and HMS 1 the market trades between players are the career's own.</summary>
    public sealed class WalletTests
    {
        private static long Purse(Town town, string column) =>
            town.Sql("SELECT " + column + " FROM traders WHERE name = 'Budi'");

        [Fact] public void TheMarketSeesTheCareersCashFromTheFirstDay()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            Assert.Equal(500000, Purse(town, "cash"));
            Assert.Equal(0, Purse(town, "stock"));
            Assert.Equal(1, town.Sql("SELECT caste FROM standing s JOIN traders t ON t.id = s.trader WHERE name = 'Budi'"));

            phone.Go(phone.Map.Piles[0].Position); phone.Do("Inspect", "p0"); phone.Do("PickUp", "p0"); phone.Send();
            phone.Play(20); phone.Go(phone.Map.Home); phone.Do("SortAtHome"); phone.Send();
            phone.Play(30); phone.Go(phone.Map.Factory); phone.Do("PreviewSale"); phone.Do("ConfirmSale"); phone.Send();
            Assert.True(phone.Game.View.Cash > 500000);
            Assert.Equal((long)phone.Game.View.Cash, Purse(town, "cash"));
            Assert.Equal(0, phone.Corrections);
        }

        [Fact] public void WhatTheMarketPaidOrTookArrivesInTheCareerOnce()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            // a lot sold to another player and a contract delivery received, as server/market writes them
            town.Sql("UPDATE traders SET cash = cash + 250000, stock = stock + 40 WHERE name = 'Budi'");

            phone.Play(5); phone.Send();
            Assert.Equal(1, phone.Corrections);
            Assert.Equal(750000, phone.Game.View.Cash);
            Assert.Equal(40, phone.Game.WalletStockKg, 6);
            phone.Play(5); phone.Send();
            Assert.Equal(1, phone.Corrections);
            Assert.Equal(750000, Purse(town, "cash"));
            Assert.Equal(40, Purse(town, "stock"));

            town.Sql("UPDATE traders SET cash = cash - 700000, stock = stock - 15 WHERE name = 'Budi'");
            town.Careers.Close();   // also after a restart of the server
            phone.Reopen();
            Assert.Equal(50000, phone.Game.View.Cash);
            Assert.Equal(25, phone.Game.WalletStockKg, 6);
            Assert.Equal(25, Purse(town, "stock"));
        }

        [Fact] public void AClosedSeasonStartsTheCareerOver()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            town.Sql("UPDATE traders SET cash = cash + 900000 WHERE name = 'Budi'");
            phone.Play(5); phone.Send();
            Assert.Equal(1400000, phone.Game.View.Cash);
            // what server/market/season/closing.py does to every account
            town.Sql("UPDATE traders SET cash = 200000, stock = 20; DELETE FROM careers; DELETE FROM wallets; DELETE FROM standing");

            phone.Play(5); phone.Send();
            Assert.Equal(500000, phone.Game.View.Cash);
            Assert.Equal(0, phone.Seq);
            Assert.Equal(500000, Purse(town, "cash"));
            phone.Play(5);
            Assert.All(phone.Send().results, result => Assert.True(result.ok, result.error));
            Assert.Equal(1, phone.Seq);
        }

        [Fact] public void TwoAccountsKeepTwoPurses()
        {
            using var town = new Town();
            var budi = new Phone(town, "Budi");
            var sari = new Phone(town, "Sari");
            town.Sql("UPDATE traders SET cash = cash + 1000 WHERE name = 'Sari'");
            budi.Play(5); budi.Send(); sari.Play(5); sari.Send();
            Assert.Equal(500000, budi.Game.View.Cash);
            Assert.Equal(501000, sari.Game.View.Cash);
        }
    }
}
