"""Route blockades between Kasta 3 traders (PRD 6.3): Rp500 million for one game day, three game days between
blockades of the same target, and nobody in his first seven real days of Kasta 3 can be a target."""
from ..clock import RULES, TIME
from ..orders import MarketError, require
from .members import fields, member, mine

PROTECTED = RULES["casteThreeProtectionRealDays"] * 86400 // TIME["realSecondsPerGameHour"]


def standing(db, trader):
    row = db.execute("SELECT caste, since FROM standing WHERE trader=?", (trader,)).fetchone()
    return (row["caste"], row["since"]) if row else (1, 0)


def pay(db, trader, amount, shortage):
    paid = db.execute("UPDATE traders SET cash=cash-? WHERE id=? AND cash>=?", (amount, trader, amount))
    require(paid.rowcount, 409, shortage)


def promote(db, trader, command, quote):
    """Buys the caste package of PRD A.4 from the market account."""
    fields(command)
    caste = standing(db, trader)[0]
    require(caste < 3, 409, "Anda sudah Kasta 3.")
    pay(db, trader, RULES["promoteCost"][caste - 1], "Kas akun pasar kurang untuk paket naik kasta.")
    db.execute("INSERT OR REPLACE INTO standing VALUES(?,?,?)", (trader, caste + 1, quote["gameHour"]))
    return {"message": "Naik ke Kasta " + str(caste + 1) + "."}


def blocked(db, trader, hour):
    return db.execute("""SELECT b.id, b.until, t.name FROM blockades b JOIN traders t ON t.id=b.hirer
                         WHERE b.target=? AND b.until>?""", (trader, hour)).fetchone()


def blockade(db, trader, command, quote):
    fields(command, "name")
    own, hour = member(db, trader), quote["gameHour"]
    require(standing(db, trader)[0] == 3, 403, "Hanya anggota Kasta 3 yang dapat menyewa blokade.")
    target = db.execute("SELECT id FROM traders WHERE name=?", (str(command["name"]),)).fetchone()
    require(target and target["id"] != trader, 409, "Pilih pedagang lain sebagai target.")
    caste, since = standing(db, target["id"])
    require(caste == 3, 409, "Target harus Kasta 3.")
    require(hour - since >= PROTECTED, 409, "Pemain baru Kasta 3 dilindungi 7 hari.")
    theirs = mine(db, target["id"])
    require(not theirs or theirs["id"] != own["id"], 409, "Target adalah anggota asosiasi Anda.")
    last = db.execute("SELECT MAX(until) FROM blockades WHERE target=?", (target["id"],)).fetchone()[0]
    require(last is None or hour >= last + RULES["blockadeCooldownHours"], 409,
            "Target baru saja diblokade. Tunggu 3 hari game.")
    pay(db, trader, RULES["blockadeCost"], "Kas akun pasar kurang untuk menyewa blokade.")
    db.execute("INSERT INTO blockades(association,hirer,target,start,until) VALUES(?,?,?,?,?)",
               (own["id"], trader, target["id"], hour, hour + RULES["blockadeHours"]))
    return {"message": "Rute " + command["name"] + " diblokade 1 hari game."}


def escort(db, trader, command, quote):
    """The target hires an escort team (PRD D.2: Rp50 million per event) that opens his route again."""
    fields(command)
    active = blocked(db, trader, quote["gameHour"])
    require(active, 409, "Rute Anda tidak sedang diblokade.")
    pay(db, trader, RULES["escortCost"], "Kas akun pasar kurang untuk menyewa pengawalan.")
    db.execute("UPDATE blockades SET until=? WHERE id=?", (quote["gameHour"], active["id"]))
    return {"message": "Tim pengawal membuka rute Anda."}


def gate(db, trader, command, hour):
    """A blockaded trader cannot move goods: no lots sold or bought until the route is open."""
    if command.get("action") in ("sell", "buy") and blocked(db, trader, hour):
        raise MarketError(409, "Rute Anda sedang diblokade. Sewa pengawalan atau tunggu blokade selesai.")
