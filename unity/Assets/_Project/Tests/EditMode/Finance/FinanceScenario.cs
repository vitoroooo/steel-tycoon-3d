using Steel.Core.Economy;
using Steel.Core.Economy.Market;
using Steel.Core.Fleet;
using Steel.Core.Scrap;
using Steel.Core.State;
using Steel.Core.State.Slice;

namespace Steel.Tests.Finance
{
    internal sealed class FinanceScenario
    {
        internal readonly SliceSession Game;
        internal readonly ConfigValues Business = new ConfigValues(TestConfig.Read("Business"));
        internal readonly AiQuote Quote;
        internal readonly LocalBuyer Buyer = LocalBuyer.Read(new ConfigValues(TestConfig.Read("LocalMarket")))[0];

        internal FinanceScenario(int startingCash = 500000)
        {
            var rules = new SliceRules(TestConfig.With("City", "startCash", startingCash));
            var garage = new Garage(new MotorRules("triseda", "triseda", TestConfig.Read("Triseda")));
            Game = new SliceSession(rules, new SliceWorld(TestConfig.Read("Piles")),
                new ScrapPricing(new ScrapConfig(TestConfig.Read("Scrap"))), garage,
                new CartRules(TestConfig.Read("Gerobak")));
            var market = new AiPriceBook(new EconomyConfig(TestConfig.Read("Economy")));
            market.Refresh(0, 1000000);
            Quote = market.Current;
        }

        internal void Gather()
        {
            var pile = Game.World.Find("p0");
            Game.Position = pile.Position;
            Game.Inspect(pile.Id); Game.PickUp(pile.Id);
            Game.Position = Game.Rules.Home; Game.SortAtHome();
        }

        internal void BuyMotor()
        {
            Game.Position = Game.Fleet.Find("triseda").Position;
            Game.BuyMotor("triseda");
        }
    }
}
