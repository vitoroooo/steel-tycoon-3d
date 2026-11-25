"""The association chat, with a word filter and reports to the operator (PRD 8.1)."""
from ..clock import RULES
from ..orders import require, whole
from .members import fields, member
from .words import clean


def say(db, trader, command, quote):
    fields(command, "text")
    own = member(db, trader)
    text = command["text"]
    require(type(text) is str and text.strip() and len(text) <= RULES["chatMaxLength"] and text.isprintable(), 400,
            "Pesan 1 sampai " + str(RULES["chatMaxLength"]) + " karakter.")
    db.execute("INSERT INTO messages(association,trader,body,game_hour) VALUES(?,?,?,?)",
               (own["id"], trader, clean(text.strip()), quote["gameHour"]))
    return {"message": "Pesan terkirim."}


def report(db, trader, command, quote):
    fields(command, "message")
    own = member(db, trader)
    line = db.execute("SELECT * FROM messages WHERE id=? AND association=?",
                      (whole(command, "message", 2**63 - 1), own["id"])).fetchone()
    require(line and line["trader"] != trader, 409, "Pesan tidak ditemukan.")
    db.execute("INSERT OR IGNORE INTO reports VALUES(?,?,?,?)", (line["id"], trader, line["trader"], line["body"]))
    return {"message": "Laporan dikirim ke pengelola pasar."}


def recent(db, association):
    rows = db.execute("""SELECT m.id, t.name, m.body AS text, m.game_hour AS hour FROM messages m
                         JOIN traders t ON t.id=m.trader WHERE m.association=? ORDER BY m.id DESC LIMIT ?""",
                      (association, RULES["chatShown"]))
    return [dict(row) for row in rows][::-1]
