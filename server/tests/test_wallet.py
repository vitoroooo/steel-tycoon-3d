from server.tests.contract_case import DAY, ContractCase


class WalletTests(ContractCase):
    """An account whose career runs on the career server (a row in wallets) has one purse, kept by that career."""

    def setUp(self):
        super().setUp()
        self.sql("INSERT INTO wallets SELECT id, cash, stock FROM traders WHERE name='Sari'")

    def test_the_free_collection_route_is_only_for_accounts_without_a_career(self):
        self.now += DAY
        self.assertEqual(self.board(self.seller)["stock"], 100)
        self.assertEqual(self.board(self.buyer)["stock"], 100 + 24)

    def test_caste_follows_the_career_and_cannot_be_bought(self):
        self.sql("UPDATE traders SET cash=5000000000")
        status, answer = self.order(self.seller, "promote")
        self.assertEqual(status, 409)
        self.assertIn("karier", answer["message"])
        self.assertEqual(self.board(self.seller)["cash"], 5000000000)
        self.assertEqual(self.order(self.buyer, "promote")[0], 200)

    def test_a_lot_moves_cash_and_stock_in_the_one_purse(self):
        self.assertEqual(self.order(self.seller, "sell", kg=10, price=self.price)[0], 200)
        lot = self.board(self.buyer)["lots"][0]["id"]
        self.assertEqual(self.order(self.buyer, "buy", lot=lot)[0], 200)
        cash, stock = self.sql("SELECT cash, stock FROM traders WHERE name='Sari'")[0]
        self.assertEqual(stock, 90)
        self.assertGreater(cash, 1000000)

    def test_a_closed_season_empties_the_careers_so_they_start_over(self):
        self.sql("INSERT INTO careers VALUES((SELECT id FROM traders WHERE name='Sari'), '{}', 0, 3, '{}')")
        self.now += 91 * 86400
        self.board(self.buyer)
        self.assertEqual(self.sql("SELECT (SELECT COUNT(*) FROM careers), (SELECT COUNT(*) FROM wallets)"), [(0, 0)])
