from server.tests.contract_case import DAY, ContractCase


class ContractTests(ContractCase):
    """Direct contracts (PRD C.2): 3 to 10 game days, money locked per delivery, 10% of the rest to cancel."""

    def test_a_contract_runs_three_to_ten_days_and_only_the_buyer_accepts(self):
        offer = dict(buyer="Hasan", kg=10, price=self.price)
        self.assertEqual(self.order(self.seller, "offer", days=2, **offer)[0], 400)
        self.assertEqual(self.order(self.seller, "offer", days=11, **offer)[0], 400)
        self.assertEqual(self.order(self.seller, "offer", days=3, **{**offer, "buyer": "Sari"})[0], 409)
        self.assertEqual(self.board(self.seller)["traders"], ["Hasan"])
        status, made = self.order(self.seller, "offer", days=3, **offer)
        self.assertEqual(status, 200)
        self.assertEqual(self.order(self.seller, "offer", days=5, **offer)[0], 409)
        self.assertEqual(self.order(self.seller, "accept", contract=made["contract"])[0], 403)
        self.assertEqual(self.board(self.buyer)["contracts"][0]["status"], "offered")
        self.assertEqual(self.order(self.buyer, "end", contract=made["contract"])[1]["penalty"], 0)
        self.assertEqual(self.board(self.buyer)["cash"], 1000000)

    def test_each_game_day_delivers_pays_the_locked_money_and_locks_the_next(self):
        self.running()
        day = 10 * self.price
        tax = (day * 2 + 50) // 100
        self.assertEqual(self.board(self.buyer)["cash"], 1000000 - day)
        self.now += DAY
        buyer, seller = self.board(self.buyer), self.board(self.seller)
        self.assertEqual((buyer["cash"], buyer["stock"]), (1000000 - 2 * day, 110 + 24))   # he collects 24 kg a day himself
        self.assertEqual((seller["cash"], seller["stock"]), (1000000 + day - tax, 90 + 24))
        self.assertEqual(buyer["contracts"][0]["delivered"], 1)
        self.now += 5 * DAY   # nobody looks for days: the rest still settles, once
        buyer, seller = self.board(self.buyer), self.board(self.seller)
        self.assertEqual(buyer["cash"], 1000000 - 3 * day)
        self.assertEqual(buyer["contracts"][0]["delivered"], 3)
        self.assertEqual(seller["cash"], 1000000 + 3 * (day - tax))
        self.assertEqual((buyer["contracts"][0]["status"], buyer["contracts"][0]["locked"]), ("done", 0))
