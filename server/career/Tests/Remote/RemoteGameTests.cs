using System.IO;
using System.Threading.Tasks;
using Steel.Career.Generate;
using Steel.Career.Tests.Support;
using Steel.Services;
using Steel.Services.Remote;
using Xunit;

namespace Steel.Career.Tests.Remote
{
    /// <summary>The game's own RemoteGameService and RemoteLink against the career server, without the network between.</summary>
    public sealed class RemoteGameTests
    {
        private static RemoteGameService Join(Town town, string name, out CityLayout map, bool[] offline = null)
        {
            long account = town.Accounts.Find(town.Token(name));
            RemoteReply opened = town.Careers.Open(account);
            var link = new RemoteLink(batch => offline != null && offline[0]
                ? Task.FromException<RemoteReply>(new IOException("putus"))
                : Task.FromResult(town.Careers.Act(account, batch)), opened.seq);
            map = new CityLayout(town.Rules);
            return new RemoteGameService(town.Rules, opened, link);
        }

        /// <summary>Frames of a quarter second for the given time, on the phone and on the server's clock alike.</summary>
        private static void Play(Town town, IGameService game, double seconds)
        {
            for (double left = seconds; left > 0; left -= 0.25) { town.Pass(0.25); game.Advance(0.25); }
        }

        private static void SellPile(Town town, RemoteGameService game, CityLayout map, int pile)
        {
            SliceLocation at = map.Piles[pile].Position;
            game.SetPosition(at.X, at.Z); game.Inspect("p" + pile); game.PickUp("p" + pile); Play(town, game, 20);
            game.SetPosition(map.Home.X, map.Home.Z); game.SortAtHome(); Play(town, game, 20);
            game.SetPosition(map.Factory.X, map.Factory.Z); game.PreviewSale(); game.ConfirmSale(); Play(town, game, 20);
        }

        [Fact] public void PlayingThroughTheGameServiceKeepsPhoneAndServerTheSameCareer()
        {
            using var town = new Town();
            RemoteGameService game = Join(town, "Budi", out CityLayout map);
            for (int pile = 0; pile < 4; pile++) SellPile(town, game, map, pile);
            Assert.Equal(0, game.Corrections);
            Assert.Null(game.Notice);
            Assert.True(game.View.Cash > 500000);
            Assert.Equal(game.Digest, town.Careers.Open(town.Accounts.Find(town.Token("Budi", rotate: false))).digest);
        }

        [Fact] public void WhatIsDoneWhileTheServerIsAwayArrivesOnceItIsBack()
        {
            using var town = new Town();
            bool[] offline = { true };
            RemoteGameService game = Join(town, "Budi", out CityLayout map, offline);
            SliceLocation at = map.Piles[0].Position;
            game.SetPosition(at.X, at.Z); game.Inspect("p0"); game.PickUp("p0"); Play(town, game, 4);
            Assert.NotNull(game.Notice);
            double carried = game.View.CarriedKg;
            offline[0] = false; Play(town, game, 12);
            Assert.Null(game.Notice);
            Assert.Equal(carried, game.View.CarriedKg);
            Assert.True(carried > 0);
        }

        [Fact] public void StandingStillForAGameHourIsNotBeingAway()
        {
            using var town = new Town();
            RemoteGameService game = Join(town, "Budi", out _);
            Play(town, game, 700);
            Assert.Equal(0, game.Corrections);
        }

        [Fact] public void APhoneThatFallsBehindIsBroughtBackToTheServersTime()
        {
            using var town = new Town();
            RemoteGameService game = Join(town, "Budi", out _);
            Play(town, game, 10);
            town.Pass(40);                       // frames the phone dropped, or the game in the background
            Play(town, game, 10);
            Assert.Equal(1, game.Corrections);
            Play(town, game, 60);
            Assert.Equal(1, game.Corrections);   // one correction heals it; it does not come back every batch
        }

        [Fact] public void TheGeneratedHalfOfTheGameServiceIsUpToDate()
        {
            string folder = Path.Combine(Town.Resources, "..", "Services", "Remote", "Generated");
            foreach (var file in RemoteServiceWriter.Files())
                Assert.True(file.Value == File.ReadAllText(Path.Combine(folder, file.Key)),
                    file.Key + " is stale: run server/career with 'generate'.");
        }
    }
}
