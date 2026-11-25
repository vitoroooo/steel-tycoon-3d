import unittest
from copy import deepcopy
from agents.player import Player
from config import load
from report.cohorts import milestone, survivors
from report.metrics import summarize


class MetricsTests(unittest.TestCase):
    def setUp(self):
        self.players = [Player(i, kind, 5, 0, 0, 1) for i, kind in
                        enumerate(('aktif', 'aktif', 'aktif', 'santai'))]
        for p in self.players:
            p.milestones['kasta2'] = 45 if p.kind == 'santai' else 12
        self.result = dict(players=self.players, config=load('fixed'), waits=[],
                           opening_money=100, profits=[dict(caste=k, profit=1)
                                                      for k in (1, 2, 3)],
                           daily=[dict(day=d, kasta1=75, kasta2=20, kasta3=5,
                                       ibn=10000, money=100, ai_share=.1)
                                  for d in range(1, 91)])

    def test_survivor_cohort_excludes_churn_and_newcomers(self):
        self.players[0].active = False
        self.players[1].born = 1
        self.assertEqual(survivors(self.players, 'aktif'), [self.players[2]])
        self.assertEqual(milestone([], 'kasta2'), dict(median=None, reached=0, cohort=0))

    def test_nonachievers_are_censored_not_discarded(self):
        self.players[0].milestones.clear()
        self.players[1].milestones.clear()
        value = milestone(survivors(self.players, 'aktif'), 'kasta2')
        self.assertEqual(value, dict(median=None, reached=1, cohort=3))
        self.assertFalse(summarize(self.result)['checks']['progression'])

    def test_all_four_checks_and_strict_limits(self):
        self.assertTrue(all(summarize(self.result)['checks'].values()))
        self.result['daily'][59]['ai_share'] = .5
        self.assertFalse(summarize(self.result)['checks']['ai'])
        self.result['daily'][59]['ai_share'] = .1
        self.result['daily'][60]['ai_share'] = 1
        self.assertTrue(summarize(self.result)['checks']['ai'])
        self.result['daily'][-1]['kasta3'] = 0
        self.assertFalse(summarize(self.result)['checks']['progression'])

    def test_no_input_days_are_na_but_all_na_cannot_pass(self):
        self.result['daily'][0]['ai_share'] = None
        self.assertTrue(summarize(self.result)['checks']['ai'])
        for day in self.result['daily']:
            day['ai_share'] = None
        self.assertFalse(summarize(self.result)['checks']['ai'])

    def test_inflation_ignores_first_two_weeks_not_third(self):
        for day in self.result['daily']:
            day['money'] = 200 if day['day'] >= 14 else 100
        self.assertTrue(summarize(self.result)['checks']['inflation'])
        for day in self.result['daily']:
            day['money'] = 220 if day['day'] >= 21 else day['money']
        self.assertFalse(summarize(self.result)['checks']['inflation'])

    def test_empty_observations_do_not_pass(self):
        result = deepcopy(self.result)
        result.update(players=[], daily=[], profits=[])
        self.assertEqual(summarize(result)['checks'],
                         dict(profit=False, ai=False, inflation=False, progression=False))
