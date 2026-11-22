using Steel.Career.Tests.Support;
using Xunit;

namespace Steel.Career.Tests.Careers
{
    /// <summary>The phone's guess and the server's result stay the same career over a long sitting, across game hours.</summary>
    public sealed class LongPlayTests
    {
        [Fact] public void ThreeGameHoursOfCollectingAndSellingNeedNoCorrection()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            double began = phone.Game.View.GameHours;
            for (int round = 0; round < 6; round++)
            {
                string pile = "p" + round;
                phone.Go(phone.Map.Piles[round].Position); phone.Do("Inspect", pile); phone.Do("PickUp", pile);
                phone.Send(); Idle(phone, 60);
                phone.Go(phone.Map.Home); phone.Do("SortAtHome");
                phone.Send(); Idle(phone, 150);
                phone.Go(phone.Map.Factory); phone.Do("PreviewSale"); phone.Do("ConfirmSale");
                phone.Send(); Idle(phone, 150);
            }
            Assert.Equal(0, phone.Corrections);
            Assert.True(phone.Game.View.GameHours - began > 3);
            Assert.True(phone.Game.View.Cash > 500000);
            Assert.Equal(phone.Game.Digest(), phone.Reopen().digest);
            Assert.Equal(phone.Last.digest, phone.Game.Digest());
        }

        /// <summary>Walking about with nothing to say: the phone still reports where it is every few seconds.</summary>
        private static void Idle(Phone phone, double seconds)
        {
            for (double left = seconds; left > 0; left -= 3)
            {
                phone.Play(3);
                phone.Do("SetPosition", phone.Game.View.PlayerX, phone.Game.View.PlayerZ);
                phone.Send();
            }
        }
    }
}
