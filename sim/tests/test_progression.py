import unittest
from agents.player import Player
from agents.progression import eligible, progress
from config import load


class ProgressionTests(unittest.TestCase):
    def setUp(self):
        self.c = load()
        self.p = Player(0, 'aktif', 5, 0, 0, 400000000, reputation=30, vehicle=2)

    def test_upgrade_requires_completed_permit_and_includes_land(self):
        p = self.p
        self.assertTrue(eligible(p, self.c))
        progress(p, 0, self.c)
        self.assertEqual(p.caste, 1)
        progress(p, 1, self.c)
        self.assertEqual(p.caste, 2)
        self.assertEqual(p.cash, 175000000)
        self.assertEqual(p.assets, 210000000)
        self.assertEqual(p.milestones['kasta2'], 2)

    def test_missing_cash_reputation_and_permit_fee(self):
        p = self.p
        for cash, rep in ((0, 30), (400000000, 29), (250000000, 30)):
            p.cash, p.reputation = cash, rep
            progress(p, 0, self.c)
            self.assertEqual(p.caste, 1)
        self.assertNotIn('kasta2', p.milestones)

    def test_smelter_requires_amdal_before_package(self):
        p = self.p
        p.caste, p.cash, p.reputation, p.roof = 2, 4000000000, 70, True
        progress(p, 0, self.c)
        self.assertEqual(p.caste, 2)
        progress(p, 1, self.c)
        self.assertEqual(p.caste, 3)
        self.assertEqual(p.cash, 300000000)
        self.assertEqual(p.assets, 2950000000)
        self.assertFalse(eligible(p, self.c))
