import json
import sqlite3
import subprocess
import sys
import tempfile
import unittest
from contextlib import closing
from pathlib import Path
from queue import Queue
from threading import Thread
from urllib.error import HTTPError
from urllib.request import Request, urlopen
from uuid import uuid4


class LocalMarket(unittest.TestCase):
    def setUp(self):
        directory = tempfile.TemporaryDirectory()
        self.addCleanup(directory.cleanup)
        self.db = Path(directory.name) / "test.sqlite3"
        self.cli = [sys.executable, "-m", "server.market", "--db", str(self.db)]
        self.root = Path(__file__).resolve().parents[2]
        self.process = None
        self.addCleanup(self.stop)
        self.start()

    def start(self):
        self.process = subprocess.Popen(self.cli + ["serve", "--port", "0"], cwd=self.root,
                                        stdout=subprocess.PIPE, text=True)
        ready = Queue()
        stream = self.process.stdout
        Thread(target=lambda: ready.put(stream.readline()), daemon=True).start()
        port = int(ready.get(timeout=10).strip().rsplit(":", 1)[-1])
        self.url = f"http://127.0.0.1:{port}/v1/"

    def stop(self):
        if self.process is not None:
            self.process.kill()
            self.process.wait(timeout=5)
            self.process.stdout.close()
            self.process = None

    def trader(self, name, *options):
        return subprocess.check_output(self.cli + ["trader", name, *options], cwd=self.root, text=True).strip()

    def request(self, token, body=None):
        headers = {"Authorization": "Bearer " + token, "Content-Type": "application/json"}
        data = None if body is None else json.dumps(body).encode()
        req = Request(self.url + ("market" if data is None else "orders"), data=data, headers=headers)
        try:
            response = urlopen(req, timeout=10)
        except HTTPError as error:
            response = error
        with response:
            return response.status, json.load(response)

    def board(self, token):
        status, board = self.request(token)
        self.assertEqual(status, 200)
        return board

    def order(self, token, action, *, key=None, **fields):
        return self.request(token, {"key": key or uuid4().hex, "action": action, **fields})

    def sell(self, seller, kg):
        start = self.board(seller)
        price = start["reference"]
        status, result = self.order(seller, "sell", kg=kg, price=price)
        self.assertEqual(status, 200)
        return result["lot"], kg * price

    def sql(self, statement):
        with closing(sqlite3.connect(self.db)) as db, db:
            return db.execute(statement).fetchall()
