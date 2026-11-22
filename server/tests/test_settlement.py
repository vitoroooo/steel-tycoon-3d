from server.tests.support import LocalMarket


class SettlementTests(LocalMarket):
    def setUp(self):
        super().setUp()
        self.seller = self.trader("Sari")
        self.buyer = self.trader("Hasan")

    def test_crash_restart_and_replay_receipts(self):
        s0, b0 = self.board(self.seller), self.board(self.buyer)
        sell = dict(key="order0001", action="sell", kg=5, price=s0["reference"])
        placed = self.request(self.seller, sell)
        self.assertEqual(placed[0], 200)
        self.assertEqual(self.request(self.seller, sell), placed)
        self.assertEqual(self.request(self.seller, {**sell, "kg": 6})[0], 409)
        lot = placed[1]["lot"]
        self.assertEqual(self.board(self.seller)["stock"], s0["stock"] - 5)
        # Keys belong to one trader, so the buyer can use the same spelling.
        buy = dict(key="order0001", action="buy", lot=lot)
        paid = self.request(self.buyer, buy)
        self.assertEqual(paid[0], 200)
        gross = 5 * s0["reference"]
        tax = (gross * 2 + 50) // 100
        self.assertEqual((paid[1]["gross"], paid[1]["tax"]), (gross, tax))
        self.stop()
        self.start()
        self.assertEqual(self.request(self.buyer, buy), paid)
        self.assertEqual(self.request(self.seller, sell), placed)
        s1, b1 = self.board(self.seller), self.board(self.buyer)
        self.assertEqual((b1["cash"], s1["cash"]), (b0["cash"] - gross, s0["cash"] + gross - tax))
        self.assertEqual((b1["stock"], s1["stock"]), (b0["stock"] + 5, s0["stock"] - 5))
        self.assertEqual(s1["lots"], [])
        self.assertEqual(self.sql("SELECT lot,gross,tax FROM trades"), [(lot, gross, tax)])

    def test_storage_failure_rolls_back_every_effect(self):
        lot, _ = self.sell(self.seller, 2)
        s0, b0 = self.board(self.seller), self.board(self.buyer)
        self.sql("""CREATE TRIGGER interrupt BEFORE INSERT ON trades
                    BEGIN SELECT RAISE(ABORT,'simulated storage error'); END""")
        buy = dict(action="buy", key="atomic001", lot=lot)
        self.assertEqual(self.request(self.buyer, buy)[0], 503)
        self.assertEqual(self.board(self.seller), s0)
        self.assertEqual(self.board(self.buyer), b0)
        self.assertEqual(self.sql("SELECT count(*) FROM requests WHERE key='atomic001'"), [(0,)])
        self.sql("DROP TRIGGER interrupt")
        self.assertEqual(self.request(self.buyer, buy)[0], 200)

    def test_cancel_restores_escrow_only_once(self):
        before = self.board(self.seller)
        lot, _ = self.sell(self.seller, 5)
        self.assertEqual(self.order(self.buyer, "cancel", lot=lot)[0], 403)
        self.assertEqual(self.order(self.seller, "buy", lot=lot)[0], 409)
        self.assertEqual(self.order(self.seller, "sell", kg=1, price=before["reference"])[0], 409)
        cancel = dict(action="cancel", key="cancel001", lot=lot)
        result = self.request(self.seller, cancel)
        self.assertEqual(result[0], 200)
        self.assertEqual(self.request(self.seller, cancel), result)
        self.assertEqual(self.order(self.buyer, "buy", lot=lot)[0], 409)
        self.assertEqual(self.board(self.seller), before)
