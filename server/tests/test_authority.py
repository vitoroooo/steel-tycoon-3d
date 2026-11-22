import time

from server.market.service import dispatch
from server.tests.support import LocalMarket


class AuthorityTests(LocalMarket):
    def test_credentials_and_forged_commands(self):
        token = self.trader("Sari")
        before = self.board(token)
        self.assertEqual(self.request("")[0], 401)
        self.assertEqual(self.order("a" * 43, "buy", lot=1)[0], 401)
        for fields in ({"cash": 100000}, {"stock": 80}, {"now": int(time.time()) + 1000000},
                       {"trader": before["id"] + 1}):
            self.assertEqual(self.order(token, "sell", kg=1, price=before["reference"], **fields)[0], 400)
        fresh = self.trader("Sari", "--rotate")
        self.assertEqual(self.request(token)[0], 401)
        self.assertEqual(self.board(fresh), before)

    def test_stock_and_prices(self):
        trader = self.trader("Hasan")
        before = self.board(trader)
        for price in (before["minimum"] - 1, before["maximum"] + 1):
            self.assertEqual(self.order(trader, "sell", kg=1, price=price)[0], 409)
        for kg in (0, -1, True, 0.5, 2**64):
            self.assertEqual(self.order(trader, "sell", kg=kg, price=before["reference"])[0], 400)
        self.assertEqual(self.order(trader, "sell", kg=before["stock"] + 1, price=before["reference"])[0], 409)
        self.assertEqual(self.board(trader), before)

    def test_server_clock_caps_idle_collection_per_trader(self):
        active, idle = self.trader("Sari"), self.trader("Rian")
        start = self.sql("SELECT MAX(accrued_at) FROM traders")[0][0]
        def read(token, seconds):
            return dispatch(self.db, token, None, False, lambda: start + seconds)[1]
        base = read(idle, 0)
        for hours in range(1, 73):
            read(active, 600 * hours)
        late = read(idle, 72 * 600)
        self.assertEqual(late["stock"] - base["stock"], 48)
        self.assertEqual(late["gameHour"] - base["gameHour"], 72)
        self.assertNotEqual(late["ibn"], base["ibn"])
        self.assertEqual(read(idle, 600), late)
        self.assertEqual(read(idle, 72 * 600 + 599)["stock"], late["stock"])
        self.assertEqual(read(idle, 73 * 600)["stock"] - late["stock"], 1)

    def test_price_band_rechecked_on_purchase(self):
        seller, buyer = self.trader("Sari"), self.trader("Citra")
        initial = self.board(seller)
        placed = self.order(seller, "sell", kg=1, price=initial["minimum"])
        self.assertEqual(placed[0], 200)
        # Drive only the internal clock; the HTTP protocol accepts no client timestamps.
        start = self.sql("SELECT opened_at FROM city")[0][0]
        later = lambda: start + 15 * 86400
        status, board = dispatch(self.db, buyer, None, False, later)
        self.assertEqual(status, 200)
        self.assertGreater(board["minimum"], initial["minimum"])
        buy = dict(action="buy", lot=placed[1]["lot"], key="stale0001")
        self.assertEqual(dispatch(self.db, buyer, buy, True, later)[0], 409)
        self.assertEqual(self.sql("SELECT status FROM lots"), [("open",)])
