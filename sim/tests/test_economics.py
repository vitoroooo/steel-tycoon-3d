import unittest
from config import load
from model.economics import ibn, margin, contraband_ev, survival


class EconomicsTests(unittest.TestCase):
    def setUp(self):
        self.c = load()

    def test_ibn_formula_and_supply_clamps(self):
        c = self.c['ibn']
        self.assertAlmostEqual(ibn(0, 1, c['supply_target'], c), 10000)
        self.assertAlmostEqual(ibn(15, 1.03, 0, c), 10000 * 1.15 * 1.03 * 1.05)
        self.assertAlmostEqual(ibn(0, 1, 100 * c['supply_target'], c), 9500)

    def test_margins_and_zero_trade(self):
        self.assertEqual(margin(3300, 4700, 150), 1250)
        self.assertAlmostEqual(margin(5500, 10000, 2000, .1), 1888.8888889)
        self.assertAlmostEqual(margin(4700, 5500, 150, .03) * .97, 489.5)
        self.assertEqual(margin(0, 0), 0)
        self.assertEqual(margin(100, 100, tax=.02), -2)

    def test_invalid_loss(self):
        for loss in (-.1, 1, 2):
            with self.assertRaises(ValueError):
                margin(100, 200, loss=loss)

    def test_contraband_uses_cost_and_confiscated_revenue(self):
        c = self.c['contraband']
        self.assertAlmostEqual(contraband_ev(4, 0, c) / 1400, 1.457142857)
        c = load('fixed')['contraband']
        self.assertAlmostEqual(contraband_ev(c['multipliers'][0], 15, c) / 1400, 1.3)
        self.assertLess(contraband_ev(c['multipliers'][0], 50, c), 0)

    def test_retention_anchors(self):
        for age, expected in self.c['retention']:
            self.assertAlmostEqual(survival(age, self.c['retention']), expected)
