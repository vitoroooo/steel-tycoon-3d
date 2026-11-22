import unittest
from agents.mobile import credit_sale
from agents.player import Player
from agents.progression import progress
from config import load
from market.quotes import npc_quote
from model.finance import close_day
from model.operations import operate
from model.simulation import run


class MobileEconomyTests(unittest.TestCase):
    def setUp(self):
        self.c = load('mobile')
        self.p = Player(0, 'aktif', 5, 0, 0, 500000)
        self.flows = dict(ai=0, player=0, ingot=0)

    def test_mobile_uses_the_same_acceptance_bounds(self):
        self.assertEqual(self.c['targets'], load('fixed')['targets'])
        self.assertEqual(self.c['upgrade']['rep'][0], 30)
        self.assertEqual(self.c['roof'], 260000000)
        self.assertEqual(self.c['free_speed'], .4)

    def test_first_sale_funds_starter_motor_only_when_a_buyer_fills(self):
        self.c.update(agents=10, days=1)
        result = run(self.c)
        self.assertEqual(result['opening_money'], 10 * 500000)
        for p in result['players']:
            self.assertEqual((p.milestones['first_sale'], p.milestones['triseda']), (1, 1))
        self.c['npc_caps_early'] = [0, 0, 0]
        p = run(self.c)['players'][0]
        self.assertEqual((p.milestones, p.reputation, p.vehicle), ({}, 0, 0))

    def test_grant_is_a_one_time_transfer_excluded_from_trading_profit(self):
        p = self.p
        credit_sale(p, 12, 0, self.c)
        credit_sale(p, 2888, 1, self.c)
        self.assertEqual((p.cash, p.profit, p.quarter, p.reputation), (12500000, 0, 0, 30))

    def test_permit_then_lease_then_cash_check_and_tools(self):
        p = self.p
        p.cash, p.vehicle, p.reputation = 285000000, 2, 30
        progress(p, 0, self.c)
        self.assertEqual((p.cash, p.caste, p.lease_until), (270000000, 1, 0))
        progress(p, 0, self.c)
        self.assertEqual(p.caste, 1)
        p.cash -= 1
        progress(p, 1, self.c)
        self.assertEqual((p.caste, p.lease_until), (1, 0))
        p.cash += 1
        progress(p, 1, self.c)
        self.assertEqual((p.caste, p.cash, p.assets), (2, 75000000, 175000000))
        self.c['maintenance'], p.workload = 0, 0
        close_day(p, 1, self.c)
        self.assertEqual((p.cash, p.profit), (75000000, -4000000))
        progress(p, 6, self.c)
        self.assertEqual((p.cash, p.lease_until), (55000000, 11))

    def test_expired_lease_blocks_receipts_but_owned_stock_can_ship(self):
        p = self.p
        p.caste, p.stock, p.stock_cost = 2, 14000, 14000000
        self.c['prices']['bandar'] = 1
        p.mark('first_sale', 0)
        operate([p], 5500, self.c, self.flows, 1, [])
        self.assertEqual((p.stock, p.cash, self.flows['ai']), (14000, 500000, 0))
        p.stock, p.stock_cost = 18000, 18000000
        operate([p], 5500, self.c, self.flows, 1, [])
        self.assertAlmostEqual(p.stock, 3000)
        self.assertAlmostEqual(p.stock_cost, 3000000)
        self.assertAlmostEqual(p.cash, 500000 + 15000 * .97 * 5500 - 3000000)

    def test_mobile_npc_quote_compares_freight_after_pricing(self):
        gross, freight = npc_quote(1, 5500, self.c)
        self.assertEqual(gross, 5500 * .86)
        self.assertGreater(freight, 0)
        self.assertEqual(npc_quote(1, 1, self.c), (.7, 0))
