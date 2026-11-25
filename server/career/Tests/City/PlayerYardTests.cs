using System.Linq;
using Steel.Career.Tests.Support;
using Steel.Services;
using Xunit;

namespace Steel.Career.Tests.City
{
    /// <summary>PRD 5.1/5.2: a collector sells his sorted load at another player's yard, at the price on its board.</summary>
    public sealed class PlayerYardTests
    {
        private static long Cash(Town town, string name) => town.Sql("SELECT cash FROM traders WHERE name = '" + name + "'");

        /// <summary>Sari picks up a pile, sorts it at home and walks to the board on the plot.</summary>
        private static PlayerYardView Carry(Phone sari)
        {
            sari.Go(sari.Map.Piles[0].Position); sari.Do("Inspect", "p0"); sari.Do("PickUp", "p0"); sari.Send(); sari.Play(20);
            sari.Go(sari.Map.Home); sari.Do("SortAtHome"); sari.Send(); sari.Play(20);
            PlayerYardView board = sari.Game.PlayerYards.Single();
            sari.Do("SetPosition", board.X, board.Z); sari.Send();
            return sari.Game.PlayerYards.Single();
        }

        [Fact] public void ACollectorSellsHisLoadAtAnotherPlayersBoard()
        {
            using var town = new Town();
            var budi = new Phone(town, "Budi");
            Bandar.Open(town, "Budi", 300e6, 5200, 4100, 3000, 2500);
            var sari = new Phone(town, "Sari");
            Assert.Empty(budi.Reopen().yards);                       // nobody sees his own yard
            PlayerYardView board = Carry(sari);
            Assert.Equal(("Budi", 2, 5200d), (board.Owner, board.Plot, board.Bids[0]));
            Assert.True(board.Offer > 0, board.Why);

            double before = sari.Game.View.Cash, carried = sari.Game.View.CarriedKg;
            sari.Do("SellToPlayerYard", 2); sari.Send();
            Assert.Equal(0, sari.Corrections);
            Assert.Equal(before + board.Offer, sari.Game.View.Cash, 6);
            Assert.Equal(0, sari.Game.View.CarriedKg);
            Assert.Equal((long)(before + board.Offer), Cash(town, "Sari"));
            Assert.Equal((long)(300e6 - board.Offer), Cash(town, "Budi"));

            town.Careers.Close();                                    // the buyer was not playing: it is on disk
            budi.Reopen();
            Assert.Equal(300e6 - board.Offer, budi.Game.View.Cash, 6);
            Assert.Equal(carried, Enumerable.Range(0, 4).Sum(grade => budi.Game.YardInfo(grade).Stock), 6);
            Assert.Equal(60000 - carried, sari.Send().yards.Single().roomKg, 6);
        }

        [Fact] public void AYardThatCannotPayTurnsTheLoadAway()
        {
            using var town = new Town();
            var budi = new Phone(town, "Budi");
            Bandar.Open(town, "Budi", 100, 5200, 4100, 3000, 2500);
            var sari = new Phone(town, "Sari");
            Carry(sari);
            double before = sari.Game.View.Cash, carried = sari.Game.View.CarriedKg;
            sari.Do("SellToPlayerYard", 2);
            var answer = sari.Send();
            Assert.Contains("Kas bandar", answer.results.Last().error);
            Assert.Equal((before, carried), (sari.Game.View.Cash, sari.Game.View.CarriedKg));   // the server's career replaced the guess
            Assert.Equal(100, Cash(town, "Budi"));
        }

        [Fact] public void TheLoadMustBeAtTheBoardAndTheBoardMustBuy()
        {
            using var town = new Town();
            var budi = new Phone(town, "Budi");
            var sari = new Phone(town, "Sari");
            Assert.Empty(sari.Last.yards);                           // Budi has no yard yet
            Bandar.Open(town, "Budi", 300e6, 0, 0, 0, 0);
            Assert.Empty(sari.Send().yards);                         // a yard without prices is not on the board
            Bandar.Open(town, "Budi", 300e6, 5200, 4100, 3000, 2500);
            Assert.Single(sari.Send().yards);
            Carry(sari);
            sari.Go(sari.Map.Home); sari.Do("SellToPlayerYard", 2);
            Assert.Contains("Dekati", sari.Send().results.Last().error);
            sari.Queue("SellToPlayerYard", 3);
            Assert.False(sari.Send().results.Last().ok);
            Assert.Equal(300000000, Cash(town, "Budi"));
        }
    }
}
