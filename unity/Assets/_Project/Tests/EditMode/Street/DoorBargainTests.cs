using NUnit.Framework;
using Steel.Core.Trade.Street;

namespace Steel.Tests.Street
{
    /// <summary>Bargaining at a door: a fair offer strikes a deal, a low one costs patience, an insult moves nothing.</summary>
    public sealed class DoorBargainTests
    {
        private const string OneHouse = "{\"count\":1,\"s0x\":0,\"s0z\":0,\"s0kind\":0,\"s0place\":0}";
        private StreetBlock block;
        private StreetSeller seller;
        private KindRules house;

        [SetUp] public void Start()
        {
            block = new StreetBlock(TestConfig.With("StreetTrade", "spread", 0), OneHouse);
            seller = block.Sellers[0]; house = seller.Habits;
        }

        [Test]
        public void AnOfferAtTheFloorIsTakenAndBecomesThePrice()
        {
            Assert.AreEqual(house.Ask, seller.Ask, 1e-9);
            Assert.AreEqual(BargainReply.Deal, seller.Offer(house.Floor));
            Assert.AreEqual(house.Floor, seller.Ask, 1e-9);
            Assert.AreEqual(BargainReply.Final, seller.Offer(house.Floor * .99), "A struck price is not reopened");
            Assert.AreEqual(house.Floor, seller.Ask, 1e-9);
        }

        [Test]
        public void ALowOfferBringsThePriceDownPartOfTheWayUntilPatienceRunsOut()
        {
            double low = house.Floor * .95, ask = house.Ask;
            for (int left = house.Patience - 1; left >= 0; left--)
            {
                var reply = seller.Offer(low);
                ask -= (ask - house.Floor) * .4;
                Assert.AreEqual(left == 0 ? BargainReply.Final : BargainReply.Counter, reply);
                Assert.AreEqual(ask, seller.Ask, 1e-9); Assert.AreEqual(left, seller.Patience);
            }
            Assert.AreEqual(BargainReply.Final, seller.Offer(house.Floor), "Once the price is final only the asked price is taken");
            Assert.Greater(seller.Ask, house.Floor);
            Assert.AreEqual(BargainReply.Deal, seller.Offer(seller.Ask));
        }

        [Test]
        public void AnInsultingOfferCostsTwoPatienceAndMovesNothing()
        {
            Assert.AreEqual(BargainReply.Offended, seller.Offer(house.Ask * .5));
            Assert.AreEqual(house.Ask, seller.Ask, 1e-9);
            Assert.AreEqual(house.Patience - 2, seller.Patience);
        }

        [Test]
        public void TheSameRoundAlwaysOffersTheSameGoodsAndANewRoundRestocks()
        {
            var again = new StreetBlock(TestConfig.With("StreetTrade", "spread", 0), OneHouse).Sellers[0];
            Assert.AreEqual(seller.Stock.GrossKg, again.Stock.GrossKg);
            Assert.AreEqual(seller.ClaimedKg, again.ClaimedKg);
            Assert.That(seller.Stock.GrossKg, Is.InRange(house.MinKg, house.MaxKg));
            Assert.GreaterOrEqual(seller.ClaimedKg, seller.Stock.GrossKg, "A guess never runs under the true weight");
            seller.Offer(house.Ask * .5);
            block.Advance(block.Rules.RestockHours);
            Assert.AreEqual(house.Patience, seller.Patience); Assert.IsFalse(seller.Weighed);
        }
    }
}
