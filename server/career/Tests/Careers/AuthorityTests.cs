using Steel.Career.Tests.Support;
using Steel.Services.Remote;
using Xunit;

namespace Steel.Career.Tests.Careers
{
    /// <summary>The server's career is the career (PRD 12): a phone only guesses, and a wrong guess is replaced.</summary>
    public sealed class AuthorityTests
    {
        /// <summary>Picks up, sorts and sells the first pile, sending each step as it happens.</summary>
        private static RemoteReply[] SellFirstPile(Phone phone)
        {
            var replies = new RemoteReply[3];
            phone.Go(phone.Map.Piles[0].Position); phone.Do("Inspect", "p0"); phone.Do("PickUp", "p0");
            replies[0] = phone.Send(); phone.Play(20);
            phone.Go(phone.Map.Home); phone.Do("SortAtHome");
            replies[1] = phone.Send(); phone.Play(30);
            phone.Go(phone.Map.Factory); phone.Do("PreviewSale"); phone.Do("ConfirmSale");
            replies[2] = phone.Send();
            return replies;
        }

        [Fact] public void AnHonestPhoneAndTheServerAgreeSoNoCareerIsSentBack()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            Assert.Equal(500000, phone.Game.View.Cash);
            foreach (RemoteReply reply in SellFirstPile(phone))
            {
                Assert.All(reply.results, result => Assert.True(result.ok, result.error));
                Assert.Null(reply.save);
            }
            Assert.True(phone.Game.View.Cash > 500000);
            Assert.Equal(phone.Game.Digest(), phone.Last.digest);
            Assert.Equal(0, phone.Corrections);
        }

        [Fact] public void AClaimTheServerDoesNotShareIsAnsweredWithTheServersCareer()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            phone.Queue("ConfirmSale");
            RemoteReply reply = phone.Send("999999999|1|0|0|0|0|0|0|0|9");
            Assert.False(reply.results[0].ok);
            Assert.NotNull(reply.save);
            Assert.Equal(1, phone.Corrections);
            Assert.Equal(500000, phone.Game.View.Cash);
        }

        [Fact] public void ACallTheRulesRefuseChangesNothingAndSaysWhy()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            phone.Go(phone.Map.Factory); phone.Queue("Inspect", "p0");
            RemoteReply reply = phone.Send();
            Assert.False(reply.results[1].ok);
            Assert.Contains("Dekati", reply.results[1].error);
            Assert.Equal(0, phone.Game.View.CarriedKg);
        }

        [Fact] public void EachAccountHasItsOwnCareer()
        {
            using var town = new Town();
            Phone budi = new Phone(town, "Budi"), sari = new Phone(town, "Sari");
            SellFirstPile(budi);
            sari.Send();
            Assert.Equal(500000, sari.Game.View.Cash);
            Assert.True(budi.Game.View.Cash > 500000);
        }
    }
}
