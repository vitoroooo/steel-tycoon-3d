"""Direct contracts between two traders (PRD C.2): a fixed weight at a fixed price every game day for 3 to 10 game days.
The buyer's money for the next delivery is locked; either side may end the contract for 10% of what is left of it."""
from ..clock import RULES
from ..orders import check_price, require, whole

DAY = 24


def find(db, command, trader):
    require(set(command) == {"action", "contract"}, 400, "Perintah kontrak keliru.")
    row = db.execute("SELECT * FROM contracts WHERE id=? AND status IN ('offered','active')",
                     (whole(command, "contract", 2**63 - 1),)).fetchone()
    require(row, 409, "Kontrak sudah selesai, batal, atau tidak ada.")
    require(trader in (row["seller"], row["buyer"]), 403, "Kontrak ini milik pedagang lain.")
    return row


def offer(db, trader, command, quote):
    require(set(command) == {"action", "buyer", "kg", "price", "days"}, 400, "Kolom kontrak keliru.")
    kg, price = whole(command, "kg", RULES["maxLotKg"]), whole(command, "price", 2**31 - 1)
    days = whole(command, "days", RULES["contractMaxDays"])
    require(days >= RULES["contractMinDays"], 400, "Kontrak berjalan 3 sampai 10 hari game.")
    check_price(quote, price)
    buyer = db.execute("SELECT id FROM traders WHERE name=?", (str(command["buyer"]),)).fetchone()
    require(buyer and buyer["id"] != trader, 409, "Pilih pedagang lain sebagai pembeli.")
    busy = db.execute("SELECT 1 FROM contracts WHERE seller=? AND buyer=? AND status IN ('offered','active')",
                      (trader, buyer["id"])).fetchone()
    require(not busy, 409, "Masih ada kontrak berjalan dengan pedagang itu.")
    row = db.execute("INSERT INTO contracts(seller,buyer,kg,price,days,delivered,locked,due,status)"
                     " VALUES(?,?,?,?,?,0,0,0,'offered')", (trader, buyer["id"], kg, price, days))
    return {"contract": row.lastrowid, "message": "Tawaran kontrak dikirim ke pembeli."}


def lock(db, contract):
    """Sets the buyer's money for the next delivery aside. False when he cannot pay it."""
    amount = contract["kg"] * contract["price"]
    paid = db.execute("UPDATE traders SET cash=cash-? WHERE id=? AND cash>=?",
                      (amount, contract["buyer"], amount)).rowcount
    if paid:
        db.execute("UPDATE contracts SET locked=? WHERE id=?", (amount, contract["id"]))
    return bool(paid)


def accept(db, trader, command, quote):
    contract = find(db, command, trader)
    require(contract["status"] == "offered" and contract["buyer"] == trader, 403,
            "Hanya pembeli yang dapat menerima tawaran.")
    require(lock(db, contract), 409, "Kas akun pasar kurang untuk pengiriman pertama.")
    db.execute("UPDATE contracts SET status='active', due=? WHERE id=?", (quote["gameHour"] + DAY, contract["id"]))
    return {"contract": contract["id"], "message": "Kontrak berjalan. Dana pengiriman pertama dikunci."}


def close(db, contract, payer, in_full):
    """Breaks a running contract: the locked money goes back, then the one who broke it pays the other 10% of the rest."""
    db.execute("UPDATE traders SET cash=cash+? WHERE id=?", (contract["locked"], contract["buyer"]))
    left = (contract["days"] - contract["delivered"]) * contract["kg"] * contract["price"]
    penalty = left * RULES["contractPenaltyPercent"] // 100
    cash = db.execute("SELECT cash FROM traders WHERE id=?", (payer,)).fetchone()[0]
    require(cash >= penalty or not in_full, 409, "Kas akun pasar kurang untuk penalti pembatalan.")
    penalty = min(cash, penalty)   # a trader who defaults pays what he has
    other = contract["buyer"] if payer == contract["seller"] else contract["seller"]
    db.execute("UPDATE traders SET cash=cash-? WHERE id=?", (penalty, payer))
    db.execute("UPDATE traders SET cash=cash+? WHERE id=?", (penalty, other))
    db.execute("UPDATE contracts SET status='broken', locked=0, penalty=? WHERE id=?", (penalty, contract["id"]))
    return penalty


def end(db, trader, command, quote):
    contract = find(db, command, trader)
    if contract["status"] == "offered":
        db.execute("UPDATE contracts SET status='withdrawn' WHERE id=?", (contract["id"],))
        return {"contract": contract["id"], "penalty": 0, "message": "Tawaran kontrak ditarik."}
    penalty = close(db, contract, trader, True)
    return {"contract": contract["id"], "penalty": penalty, "message": "Kontrak dibatalkan. Penalti 10% sisa kontrak dibayar."}


def release(db, trader):
    """A suspended trader's contracts stop without a penalty: offers are withdrawn and locked money goes back."""
    db.execute("UPDATE contracts SET status='withdrawn' WHERE status='offered' AND ? IN (seller, buyer)", (trader,))
    for contract in db.execute("SELECT * FROM contracts WHERE status='active' AND ? IN (seller, buyer)",
                               (trader,)).fetchall():
        db.execute("UPDATE traders SET cash=cash+? WHERE id=?", (contract["locked"], contract["buyer"]))
        db.execute("UPDATE contracts SET status='broken', locked=0 WHERE id=?", (contract["id"],))


ACTIONS = {"offer": offer, "accept": accept, "end": end}
