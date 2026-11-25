import subprocess

from server.tests.support import LocalMarket


class ModerationTests(LocalMarket):
    def change(self, action):
        subprocess.run(self.cli + [action, "Sari"], cwd=self.root, check=True)

    def test_suspension_returns_escrow_once_and_persists_after_restart(self):
        seller, buyer = self.trader("Sari"), self.trader("Hasan")
        initial = self.board(seller)
        lot, _ = self.sell(seller, 5)
        self.change("suspend")
        self.change("suspend")
        self.stop()
        self.start()
        self.assertEqual(self.request(seller)[0], 403)
        self.assertFalse(self.board(buyer)["lots"])
        self.assertEqual(self.order(buyer, "buy", lot=lot)[0], 409)
        self.change("restore")
        self.assertEqual(self.board(seller)["stock"], initial["stock"])
        self.assertEqual(self.sql("SELECT operation FROM moderator_events ORDER BY id"),
                         [("suspend",), ("restore",)])

    def test_customer_count_is_distinct_and_private(self):
        seller, buyer = self.trader("Sari"), self.trader("Hasan")
        for _ in range(2):
            lot, _ = self.sell(seller, 1)
            visible = self.board(buyer)["lots"][0]
            self.assertEqual(set(visible), {"id", "seller", "name", "kg", "price"})
            self.assertEqual(self.order(buyer, "buy", lot=lot)[0], 200)
        self.assertEqual(self.board(seller)["buyers"], 1)
