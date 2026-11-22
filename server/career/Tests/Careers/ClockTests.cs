using Steel.Career.Careers;
using Steel.Career.Tests.Support;
using Xunit;

namespace Steel.Career.Tests.Careers
{
    /// <summary>Time belongs to the server: a phone can neither hurry it nor hold it back, and time away is offline time.</summary>
    public sealed class ClockTests
    {
        [Fact] public void APhoneCannotHurryTheClock()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            double start = phone.Game.ClockSeconds;
            town.Pass(5);
            phone.Game.Advance(100000);
            phone.Queue("ReadGuide");
            phone.Send();
            Assert.Equal(1, phone.Corrections);
            Assert.InRange(phone.Game.ClockSeconds - start, 5, 5 + CareerClock.Ahead);
        }

        [Fact] public void APhoneCannotHoldTheClockBack()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            double start = phone.Game.ClockSeconds;
            town.Pass(60);
            phone.Queue("ReadGuide");
            phone.Send("");
            Assert.InRange(phone.Game.ClockSeconds - start, 60 - CareerClock.Behind, 60);
        }

        [Fact] public void TimeAwayIsCountedWithTheOfflineRulesAndItsCap()
        {
            using var town = new Town();
            var phone = new Phone(town, "Budi");
            double start = phone.Game.ClockSeconds;
            town.Pass(3 * 24 * 3600);
            phone.Reopen();
            double cap = new Steel.Core.Time.TimeConfig(town.Rules.Read("Time")).OfflineCapRealSeconds;
            Assert.Equal(cap, phone.Game.ClockSeconds - start, 3);
        }
    }
}
