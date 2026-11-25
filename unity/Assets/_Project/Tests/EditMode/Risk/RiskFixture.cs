using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Steel.Core.Fleet;
using Steel.Core.Risk;
using Steel.Core.Scrap;
using Steel.Core.State;
using Steel.Core.State.Slice;
using Steel.Core.Trade.Haul;

namespace Steel.Tests.Risk
{
    /// <summary>A rich Kasta 2 yard whose owner stands at its loading post, past the newcomer's protection.</summary>
    public abstract class RiskFixture
    {
        protected const double PerKg = 5000;
        protected static readonly WorldPoint Post = new WorldPoint(50, 50);
        protected SliceSession Game;

        /// <summary>Risk.json with these numbers changed.</summary>
        protected static RiskRules Rules(params (string key, double value)[] changes)
        {
            var values = NumericJson.Parse(TestConfig.Read("Risk"));
            foreach (var change in changes) values[change.key] = change.value;
            return new RiskRules("{" + string.Join(",", values.Select(pair =>
                "\"" + pair.Key + "\":" + pair.Value.ToString("R", CultureInfo.InvariantCulture))) + "}");
        }

        protected void Open(RiskRules rules)
        {
            var slice = new SliceRules(TestConfig.With("City", "startCash", 2000000000));
            Game = new SliceSession(slice, new SliceWorld(TestConfig.Read("Piles")),
                new ScrapPricing(new ScrapConfig(TestConfig.Read("Scrap"))),
                new Garage(new MotorRules("pikap", "Pikap", TestConfig.Read("Pikap"))), new CartRules(TestConfig.Read("Gerobak")));
            Game.InitCrew(new ConfigValues(TestConfig.With("Crew", "newcomerHours", 0)));
            Game.OpenHaul(new HaulRules(TestConfig.Read("Haul"), Post, new WorldPoint(-80, 10)));
            Game.OpenRisk(rules);
            Game.Yard.bandar = Game.Yard.applied = Game.Yard.leased = true;
            Game.Yard.reputation = 60;
            Game.Position = Post;
        }

        protected static readonly (string, double)[] Certain = { ("extortChance", 1), ("extortCap", 1), ("deterMost", 1e-9) };

        /// <summary>A gang turns up at a yard with this much sold this month.</summary>
        protected void Arrive(double turnover = 500000000)
        {
            Open(Rules(Certain));
            Game.CreditProceeds(turnover, 0);
            Game.AdvanceRisk(25);
            Assert.IsTrue(Game.Risk.demand, "The gang comes on the day's roll");
        }

        protected ContrabandOffer Offer(double hour)
        {
            ContrabandOffer? offer = Game.Contraband(hour, PerKg);
            Assert.IsTrue(offer.HasValue, "No offer on this day");
            return offer.Value;
        }
    }
}
