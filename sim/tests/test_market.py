import unittest
from config import load
from agents.player import Player
from market.trading import sell, ai_supply
from model.finance import close_day
from model.simulation import run
from report.metrics import summarize


class MarketTests(unittest.TestCase):
    def setUp(self):
        self.c = load()
        self.seller = Player(0, 'aktif', 5, 0, 0, 0)
        self.buyer = Player(1, 'aktif', 5, 0, 0, 10000000, caste=2)
        self.buyer.demand = 1000
        self.flows = dict(ai=0, player=0, ingot=0)

    def test_player_trade_conserves_cash_except_tax(self):
        sell(self.seller, 1000, 0, [self.buyer], 5500, self.c, self.flows)
        self.assertEqual(self.buyer.stock, 1000)
        debit = self.buyer.stock_cost
        self.assertAlmostEqual(self.seller.cash + self.buyer.cash,
                               10000000 - debit * .02)
        self.assertEqual(self.flows['player'], 1000)

    def test_empty_book_uses_ai_and_insolvent_buyer_cannot_buy(self):
        self.buyer.cash = -1
        sell(self.seller, 1000, 0, [self.buyer], 5500, self.c, self.flows)
        self.assertEqual(self.buyer.stock, 0)
        self.assertAlmostEqual(self.seller.cash, 3850000)

    def test_ai_supply_is_district_capped(self):
        self.buyer.cash = 1e12
        self.buyer.demand = 1e9
        ai_supply([self.buyer], 5500, self.c, self.flows)
        self.assertEqual(self.flows['ai'], self.c['ai_daily_district'])

    def test_quarter_tax_on_profit_not_cash(self):
        p = self.seller
        p.cash, p.quarter = 1000000, 100000
        close_day(p, 14, self.c)
        self.assertEqual(p.cash, 995000)
        self.assertEqual(p.quarter, 0)

    def test_debt_compounds_six_game_days(self):
        p = self.seller
        p.cash = -1000000
        close_day(p, 0, self.c)
        self.assertAlmostEqual(p.cash, -1000000 * 1.02 ** 6)

    def test_empty_and_reproducible_population(self):
        c = load()
        c.update(agents=0, days=7)
        self.assertEqual(run(c)['players'], [])
        self.assertFalse(summarize(run(c))['checks']['profit'])
        self.assertFalse(summarize(run(c))['checks']['ai'])
        c.update(agents=10, days=2)
        self.assertEqual(run(c)['daily'], run(c)['daily'])
        c['days'] = 1
        players = run(c)['players']
        self.assertEqual([sum(p.kind == k for p in players) for k in c['types']], [2, 5, 3])
