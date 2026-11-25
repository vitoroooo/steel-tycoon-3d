using Steel.Career.Tests.Support;
using Steel.Services.Remote;
using Xunit;

namespace Steel.Career.Tests.Careers
{
    /// <summary>Batches on a bad connection: sent twice, out of order, or across a restart of the server.</summary>
    public sealed class DeliveryTests
    {
        private static RemoteBatch Grant(Phone phone, long seq) => new RemoteBatch
        {
            seq = seq, digest = "",
            calls = new[]
            {
                new RemoteCall { name = "SetPosition", args = new[] { CallText.Write(phone.Map.Piles[0].Position.X),
                    CallText.Write(phone.Map.Piles[0].Position.Z) } },
                new RemoteCall { name = "Inspect", args = new[] { "p0" } },
                new RemoteCall { name = "PickUp", args = new[] { "p0" } }
            }
        };

        [Fact] public void ABatchSentTwiceRunsOnce()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            RemoteReply first = phone.Resend(Grant(phone, 1)), again = phone.Resend(Grant(phone, 1));
            Assert.True(first.results[2].ok, first.results[2].error);
            Assert.True(again.results[2].ok, "The second answer is the first answer, not a refusal for picking up twice");
            Assert.Equal(1, again.seq);
        }

        [Fact] public void ABatchOutOfOrderRunsNothingAndReturnsTheCareerToStartFrom()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            RemoteReply reply = phone.Resend(Grant(phone, 5));
            Assert.Null(reply.results);
            Assert.Equal(0, reply.seq);
            Assert.NotNull(reply.save);
            Assert.Equal(0, phone.Reopen().seq);
            Assert.Equal(0, phone.Game.View.CarriedKg);
        }

        [Fact] public void TheCareerSurvivesARestartOfTheServer()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            RemoteReply before = phone.Resend(Grant(phone, 1));
            town.Careers.Close();
            Assert.Equal(before.results.Length, phone.Resend(Grant(phone, 1)).results.Length);
            phone.Reopen();
            Assert.Equal(1, phone.Seq);
            Assert.Equal(before.digest, phone.Game.Digest());
            Assert.True(phone.Game.View.CarriedKg > 0);
        }
    }
}
