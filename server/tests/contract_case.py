import sqlite3
import tempfile
import unittest
from contextlib import closing
from pathlib import Path
from uuid import uuid4

from server.market.service import dispatch
from server.market.storage import initialize, provision

DAY = 24 * 600   # one game day in real seconds


class ContractCase(unittest.TestCase):
    """Two traders with a million rupiah and 100 kg each, and a clock the test moves."""

    def setUp(self):
        directory = tempfile.TemporaryDirectory()
        self.addCleanup(directory.cleanup)
        self.db = str(Path(directory.name) / "test.sqlite3")
        initialize(self.db)
        self.seller, self.buyer = provision(self.db, "Sari"), provision(self.db, "Hasan")
        self.now = self.sql("SELECT last_time FROM city")[0][0]
        self.sql("UPDATE traders SET cash=1000000, stock=100")
        self.price = self.board(self.seller)["reference"]

    def sql(self, statement):
        with closing(sqlite3.connect(self.db)) as db, db:
            return db.execute(statement).fetchall()

    def board(self, token):
        return dispatch(self.db, token, None, False, clock=lambda: self.now)[1]

    def order(self, token, action, **fields):
        return dispatch(self.db, token, {"key": uuid4().hex, "action": action, **fields}, True, clock=lambda: self.now)

    def running(self, kg=10, days=3):
        status, made = self.order(self.seller, "offer", buyer="Hasan", kg=kg, price=self.price, days=days)
        self.assertEqual(status, 200)
        self.assertEqual(self.order(self.buyer, "accept", contract=made["contract"])[0], 200)
        return made["contract"]
