from server.tests.contract_case import DAY, ContractCase

WEEK = 7 * 86400


class BlockadeTests(ContractCase):
    """Route blockades (PRD 6.3): Kasta 3 against Kasta 3, Rp500 million a game day, three game days apart."""

    def setUp(self):
        super().setUp()
        self.sql("UPDATE traders SET cash=5000000000")
        self.order(self.seller, "found", name="Besi Tua Jaya")

    def raise_to_three(self, token):
        self.assertEqual(self.order(token, "promote")[0], 200)
        self.assertEqual(self.order(token, "promote")[0], 200)
        self.assertEqual(self.order(token, "promote")[0], 409)

    def test_the_caste_packages_cost_what_the_prd_says(self):
        self.raise_to_three(self.seller)
        board = self.board(self.seller)
        self.assertEqual((board["caste"], board["cash"]), (3, 5000000000 - 250000000 - 2950000000))

    def test_only_kasta_three_blockades_kasta_three_after_seven_real_days(self):
        self.assertEqual(self.order(self.seller, "blockade", name="Hasan")[0], 403)
        self.raise_to_three(self.seller)
        self.assertEqual(self.order(self.seller, "blockade", name="Hasan")[0], 409)   # Kasta 1 is never a target
        self.raise_to_three(self.buyer)
        self.assertEqual(self.board(self.seller)["rivals"], ["Hasan"])
        self.assertEqual(self.order(self.buyer, "blockade", name="Sari")[0], 409)     # he is in no association
        self.now += WEEK - 600
        self.assertEqual(self.order(self.seller, "blockade", name="Hasan")[0], 409)   # still a newcomer
        self.now += 600
        self.sql("UPDATE traders SET cash=499999999 WHERE name='Sari'")
        self.assertEqual(self.order(self.seller, "blockade", name="Hasan")[0], 409)
        self.sql("UPDATE traders SET cash=500000000 WHERE name='Sari'")
        self.assertEqual(self.order(self.seller, "blockade", name="Hasan")[0], 200)
        self.assertEqual(self.board(self.seller)["cash"], 0)
        blocked = self.board(self.buyer)
        self.assertEqual((blocked["blockedBy"], blocked["blockedUntil"]), ("Sari", blocked["gameHour"] + 24))
        self.order(self.buyer, "join", name="Besi Tua Jaya")
        self.sql("DELETE FROM blockades")
        self.assertEqual(self.order(self.seller, "blockade", name="Hasan")[0], 409)   # his own member

    def test_a_blockade_stops_trade_for_a_game_day_and_waits_three_days_before_the_next(self):
        self.raise_to_three(self.seller)
        self.raise_to_three(self.buyer)
        self.now += WEEK
        price = self.board(self.seller)["reference"]
        self.order(self.seller, "offer", buyer="Hasan", kg=10, price=price, days=3)
        number = self.board(self.buyer)["contracts"][0]["id"]
        self.order(self.buyer, "accept", contract=number)
        self.assertEqual(self.order(self.seller, "blockade", name="Hasan")[0], 200)
        self.assertEqual(self.order(self.buyer, "sell", kg=5, price=price)[0], 409)
        self.assertEqual(self.order(self.seller, "blockade", name="Hasan")[0], 409)
        self.now += DAY - 600
        self.assertEqual(self.board(self.buyer)["contracts"][0]["delivered"], 0)      # the delivery waits
        self.now += 600
        self.assertEqual(self.board(self.buyer)["contracts"][0]["delivered"], 1)
        self.assertEqual(self.order(self.buyer, "sell", kg=5, price=price)[0], 200)
        self.now += 3 * DAY - 600
        self.assertEqual(self.order(self.seller, "blockade", name="Hasan")[0], 409)   # three game days apart
        self.now += 600
        self.assertEqual(self.order(self.seller, "blockade", name="Hasan")[0], 200)

    def test_the_target_hires_an_escort_to_open_his_route(self):
        self.raise_to_three(self.seller)
        self.raise_to_three(self.buyer)
        self.now += WEEK
        self.assertEqual(self.order(self.buyer, "escort")[0], 409)
        self.order(self.seller, "blockade", name="Hasan")
        cash = self.board(self.buyer)["cash"]
        self.assertEqual(self.order(self.buyer, "escort")[0], 200)
        after = self.board(self.buyer)
        self.assertEqual((after["cash"], after["blockedUntil"]), (cash - 50000000, 0))
        self.assertEqual(self.order(self.buyer, "sell", kg=5, price=after["reference"])[0], 200)
