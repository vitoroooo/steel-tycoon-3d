import hashlib
import re
import secrets
import sqlite3
import time
from contextlib import closing, contextmanager

from .clock import HERE, RULES


@contextmanager
def transaction(path):
    with closing(sqlite3.connect(path, timeout=5, isolation_level=None)) as db:
        db.row_factory = sqlite3.Row
        db.execute("PRAGMA foreign_keys=ON")
        db.execute("BEGIN IMMEDIATE")
        try:
            yield db
            db.commit()
        except Exception:
            db.rollback()
            raise


def initialize(path):
    with closing(sqlite3.connect(path)) as db:
        db.execute("PRAGMA journal_mode=WAL")
        db.executescript((HERE / "schema.sql").read_text())
        now = int(time.time())
        db.execute("INSERT OR IGNORE INTO city VALUES(1,?,?)", (now, now))
        db.commit()


def digest(token):
    return hashlib.sha256(token.encode("ascii")).digest()


def provision(path, name, rotate=False):
    if not re.fullmatch(r"[\w -]{1,24}", name) or not name.strip():
        raise ValueError("Nama pedagang harus 1–24 huruf, angka, spasi, atau tanda hubung.")
    token = secrets.token_urlsafe(32)
    with transaction(path) as db:
        if rotate:
            changed = db.execute("UPDATE traders SET token_hash=? WHERE name=?", (digest(token), name))
            if not changed.rowcount:
                raise ValueError("Pedagang belum terdaftar.")
        else:
            db.execute("INSERT INTO traders(name,token_hash,cash,stock,accrued_at)"
                       " SELECT ?,?,?,?,MAX(last_time,?) FROM city",
                       (name, digest(token), RULES["initialCash"], RULES["initialStockKg"], int(time.time())))
    return token
