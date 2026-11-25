"""Associations (PRD 8.1): at most 10 traders who share a chat and may hire blockades."""
import re

from ..clock import RULES
from ..orders import require
from .words import clean


def fields(command, *names):
    require(set(command) == {"action", *names}, 400, "Kolom perintah asosiasi keliru.")


def mine(db, trader):
    return db.execute("""SELECT a.id, a.name FROM members m JOIN associations a ON a.id=m.association
                         WHERE m.trader=?""", (trader,)).fetchone()


def member(db, trader):
    own = mine(db, trader)
    require(own, 409, "Anda belum bergabung dengan asosiasi.")
    return own


def found(db, trader, command, quote):
    fields(command, "name")
    name = command["name"]
    require(type(name) is str and re.fullmatch(r"[\w -]{3,24}", name) and name.strip() == name, 400,
            "Nama asosiasi 3–24 huruf, angka, spasi, atau tanda hubung.")
    require(clean(name) == name, 400, "Nama asosiasi tidak pantas.")
    require(not mine(db, trader), 409, "Keluar dari asosiasi sekarang dahulu.")
    require(not db.execute("SELECT 1 FROM associations WHERE name=?", (name,)).fetchone(), 409,
            "Nama asosiasi sudah dipakai.")
    made = db.execute("INSERT INTO associations(name) VALUES(?)", (name,))
    db.execute("INSERT INTO members VALUES(?,?)", (trader, made.lastrowid))
    return {"message": "Asosiasi " + name + " berdiri."}


def join(db, trader, command, quote):
    fields(command, "name")
    require(not mine(db, trader), 409, "Keluar dari asosiasi sekarang dahulu.")
    target = db.execute("SELECT id,name FROM associations WHERE name=?", (str(command["name"]),)).fetchone()
    require(target, 409, "Asosiasi tidak ditemukan.")
    count = db.execute("SELECT COUNT(*) FROM members WHERE association=?", (target["id"],)).fetchone()[0]
    require(count < RULES["associationMax"], 409, "Asosiasi sudah penuh.")
    db.execute("INSERT INTO members VALUES(?,?)", (trader, target["id"]))
    return {"message": "Anda bergabung dengan " + target["name"] + "."}


def leave(db, trader, command, quote):
    fields(command)
    own = member(db, trader)
    db.execute("DELETE FROM members WHERE trader=?", (trader,))
    if not db.execute("SELECT 1 FROM members WHERE association=?", (own["id"],)).fetchone():
        db.execute("DELETE FROM messages WHERE association=?", (own["id"],))
        db.execute("DELETE FROM associations WHERE id=?", (own["id"],))
    return {"message": "Anda keluar dari " + own["name"] + "."}


def roster(db, association):
    rows = db.execute("""SELECT t.name FROM members m JOIN traders t ON t.id=m.trader
                         WHERE m.association=? ORDER BY t.name""", (association,))
    return [row["name"] for row in rows]


def directory(db):
    rows = db.execute("""SELECT a.name, COUNT(*) AS members FROM associations a JOIN members m ON m.association=a.id
                         GROUP BY a.id ORDER BY a.name LIMIT ?""", (RULES["associationsShown"],))
    return [dict(row) for row in rows]
