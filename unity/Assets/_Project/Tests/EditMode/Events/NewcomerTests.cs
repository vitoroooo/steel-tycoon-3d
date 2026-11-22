using NUnit.Framework;

namespace Steel.Tests.Events
{
    /// <summary>Appendix E's limits: no major bad event in a newcomer's first three real days, two bad events a real day at most.</summary>
    public sealed class NewcomerTests : EventFixture
    {
        private static readonly double[] Rates = { 10000, 13000, 16000 };

        private void Protect() => Crew();

        [Test] public void ANewcomerGetsNoContrabandOfferAndNoInspectionFineForThreeRealDays()
        {
            OpenMill(("inspectChance", 1), ("flashChance", 1e-12));
            Protect();
            double cash = Game.Cash;
            for (int day = 0; day < 18; day++)
            {
                Assert.IsFalse(Game.Contraband(day * 24 + 1, PerKg).HasValue, "day " + day);
                Game.AdvanceEvents(day * 24 + 1, 10000, Rates);
            }
            Assert.AreEqual(cash, Game.Cash);
            Game.AdvanceEvents(18 * 24, 10000, Rates);
            Assert.AreEqual(cash - 100000000, Game.Cash, "Fined on the first day after the protection");
        }

        [Test] public void AThirdBadEventInOneRealDayDoesNotHappen()
        {
            OpenMill(("inspectChance", 1), ("flashChance", 1e-12));
            Protect();
            double cash = Game.Cash;
            int start = 18 * 24;
            for (int day = 0; day < 6; day++) Game.AdvanceEvents(start + day * 24, 10000, Rates);
            Assert.AreEqual(cash - 2 * 100000000d, Game.Cash, "Two fines in the first real day, then none");
            Game.AdvanceEvents(start + 6 * 24, 10000, Rates);
            Assert.AreEqual(cash - 3 * 100000000d, Game.Cash, "A real day after the first, the next one may come");
        }
    }
}
