from concurrent.futures import ThreadPoolExecutor
from threading import Barrier

from server.tests.support import LocalMarket


def race(first, second):
    gate = Barrier(2, timeout=5)

    def at_once(operation):
        gate.wait()
        return operation()

    with ThreadPoolExecutor(max_workers=2) as pool:
        return list(pool.map(at_once, (first, second)))


class ContentionTests(LocalMarket):
    def test_two_buyers_one_lot(self):
        seller = self.trader("Ajeng")
        buyers = [self.trader("Budi"), self.trader("Citra")]
        before = [self.board(b) for b in buyers]
        lot, gross = self.sell(seller, 5)
        result = race(*(lambda token=b: self.order(token, "buy", lot=lot) for b in buyers))
        self.assertEqual(sorted(status for status, _ in result), [200, 409])
        for token, old, (status, _) in zip(buyers, before, result):
            after = self.board(token)
            won = status == 200
            self.assertEqual(after["cash"], old["cash"] - gross * won)
            self.assertEqual(after["stock"], old["stock"] + 5 * won)
        self.assertEqual(self.sql("SELECT count(*) FROM trades"), [(1,)])

    def test_duplicate_in_flight_with_same_key(self):
        seller, buyer = self.trader("Budi"), self.trader("Citra")
        lot, _ = self.sell(seller, 5)
        def pay():
            return self.order(buyer, "buy", key="duplicate01", lot=lot)
        first, second = race(pay, pay)
        self.assertEqual(first[0], 200)
        self.assertEqual(second, first)
        self.assertEqual(self.sql("SELECT count(*) FROM trades"), [(1,)])

    def test_simultaneous_spending_on_two_lots(self):
        sellers = [self.trader("Sari"), self.trader("Rian")]
        buyer = self.trader("Hasan")
        lots = [self.sell(s, 20)[0] for s in sellers]
        result = race(*(lambda number=n: self.order(buyer, "buy", lot=number) for n in lots))
        self.assertEqual(sorted(status for status, _ in result), [200, 409])
        remaining = self.board(buyer)
        self.assertGreaterEqual(remaining["cash"], 0)
        self.assertEqual(len(remaining["lots"]), 1)
