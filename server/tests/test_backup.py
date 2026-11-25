import sqlite3
import tempfile
import unittest
from contextlib import closing
from pathlib import Path

from server.backup.__main__ import backup
from server.market.storage import initialize, provision


class BackupTests(unittest.TestCase):
    def test_a_copy_holds_the_accounts_and_old_copies_are_dropped(self):
        with tempfile.TemporaryDirectory() as directory:
            database, folder = str(Path(directory) / "pasar.sqlite3"), Path(directory) / "backup"
            initialize(database)
            provision(database, "Sari")
            folder.mkdir()
            for stamp in ("20200101-000000", "20200102-000000"):
                (folder / ("pasar-" + stamp + ".sqlite3")).write_bytes(b"")
            copy = backup(database, folder, 2)
            with closing(sqlite3.connect(copy)) as db:
                self.assertEqual(db.execute("SELECT name FROM traders").fetchall(), [("Sari",)])
            self.assertEqual(len(list(folder.iterdir())), 2)
            self.assertFalse((folder / "pasar-20200101-000000.sqlite3").exists())
