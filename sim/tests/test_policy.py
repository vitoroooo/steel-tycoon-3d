import unittest
from agents.player import Player
from agents.progression import eligible
from config import load
from market.budget import budgets, allowance
from market.trading import sell
from model.finance import close_day
from model.operations import operate
from model.simulation import run


class PolicyTests(unittest.TestCase):
    def setUp(self):
        self.c = load('fixed')
        self.p = Player(0, 'aktif', 5, 0, 0, 0)
        self.flows = dict(ai=0, player=0, ingot=0)

    def test_budget_is_shared_daily_and_resets(self):
        caps = budgets(self.c, 14)
        self.assertEqual(allowance(caps, 1, 0, 9000), 8000)
        self.assertEqual(allowance(caps, 1, 0, 1), 0)
        self.assertEqual(caps[1][1], 8000)
        self.assertEqual(budgets(self.c, 14)[1][0], 8000)
        self.assertEqual(budgets(self.c, 13)[1][0], 500000)
        with self.assertRaises(ValueError):
            allowance(caps, 1, 0, -1)
        self.c['npc_caps_late'] = [-1, 0, 0]
        with self.assertRaises(ValueError):
            budgets(self.c, 14)

    def test_capped_npc_preserves_unsold_inventory_and_cost(self):
        caps = budgets(self.c, 14)
        self.p.npc_remaining = 8000
        left, cost = sell(self.p, 10000, 20000, [], 5500, self.c, self.flows, caps)
        self.assertEqual((left, cost), (2000, 4000))
        self.assertAlmostEqual(self.p.cash, 8000 * 3850)
        self.assertAlmostEqual(self.p.profit, 8000 * 3850 - 16000)
        self.assertEqual(sell(self.p, left, cost, [], 5500, self.c, self.flows, caps),
                         (left, cost))

    def test_npc_caps_do_not_limit_player_transfers(self):
        self.c['prices']['bandar'] = 1
        buyer = Player(1, 'aktif', 5, 0, 0, 1e8, caste=2)
        buyer.demand = 10000
        self.p.npc_remaining = 0
        sell(self.p, 1000, 0, [buyer], 5500, self.c, self.flows,
             {1: [0] * self.c['districts']})
        self.assertEqual(self.flows['player'], 1000)

    def test_small_bandars_cannot_consume_partial_ai_batch(self):
        self.p.caste, self.p.npc_remaining = 2, 4000
        caps = budgets(self.c, 14)
        before = caps[2][0]
        self.assertEqual(sell(self.p, 15000, 6000, [], 5500, self.c,
                              self.flows, caps), (15000, 6000))
        self.assertEqual(caps[2][0], before)
        self.assertEqual(self.p.cash, 0)

    def test_real_day_debt_and_maintenance(self):
        self.p.cash = -1000000
        close_day(self.p, 0, self.c)
        self.assertAlmostEqual(self.p.cash, -1020000)
        self.p.cash, self.p.assets, self.p.turnover = 1000000, 100000, 200000
        close_day(self.p, 1, self.c)
        self.assertEqual(self.p.cash, 997700)
        self.p.cash, self.p.vehicle, self.p.workload = 1000000, 2, .5
        close_day(self.p, 2, self.c)
        self.assertEqual(self.p.cash, 397700)

    def test_source_cap_inventory_and_role_goal(self):
        self.c.update(source_cap=13 / 73, free_fraction=1)
        self.p.cash, self.p.goal, self.p.reputation = 1e9, 1, 100
        self.assertFalse(eligible(self.p, self.c))
        other = Player(1, 'aktif', 5, 0, 0, 1e9)
        operate([self.p, other], 5500, self.c, self.flows, 0, [])
        self.assertAlmostEqual(self.p.turnover + other.turnover, 13 / 73 * 3850)

    def test_goal_sampling_is_reproducible(self):
        self.c.update(agents=10, days=1)
        first = run(self.c)
        second = run(self.c)
        self.assertEqual(first['daily'], second['daily'])
        self.assertEqual([p.goal for p in first['players']],
                         [p.goal for p in second['players']])

    def test_melt_roundoff_does_not_create_negative_inventory(self):
        self.p.caste, self.p.stock, self.p.stock_cost = 3, 13 / 73, 100
        operate([self.p], 5500, self.c, self.flows, 0, [])
        self.assertGreaterEqual(self.p.stock, 0)
        self.assertGreaterEqual(self.p.stock_cost, 0)
        self.assertAlmostEqual(self.flows['ingot'], 13 / 73 * .9)
