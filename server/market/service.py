import json
import re
import time

from . import contracts
from .clock import advance, prices
from .orders import MarketError, require, settle
from .storage import digest, transaction


def board(db, trader, quote):
    account = db.execute("SELECT id,name,cash,stock FROM traders WHERE id=?", (trader,)).fetchone()
    lots = db.execute("""SELECT l.id, t.id AS seller, t.name, l.kg, l.price FROM lots l
                         JOIN traders t ON t.id=l.seller WHERE l.status='open' ORDER BY l.id""")
    buyers = db.execute("""SELECT COUNT(DISTINCT buyer) FROM trades JOIN lots ON trades.lot=lots.id
                           WHERE seller=?""", (trader,)).fetchone()[0]
    return {**quote, **dict(account), "buyers": buyers, "lots": [dict(row) for row in lots],
            "contracts": contracts.listing(db, trader), "traders": contracts.partners(db, trader)}


def order(db, trader, body, quote):
    require(type(body) is dict, 400, "Format pesanan keliru.")
    key = body.get("key")
    require(type(key) is str and re.fullmatch(r"[a-zA-Z0-9_-]{8,64}", key), 400, "ID pesanan keliru.")
    command = {field: value for field, value in body.items() if field != "key"}
    payload = json.dumps(command, sort_keys=True, separators=(",", ":"), allow_nan=False)
    past = db.execute("SELECT * FROM requests WHERE trader=? AND key=?", (trader, key)).fetchone()
    if past:
        require(past["payload"] == payload, 409, "ID pesanan sudah dipakai untuk isi berbeda.")
        return past["status"], json.loads(past["receipt"])

    db.execute("SAVEPOINT settlement")
    try:
        result = contracts.ACTIONS.get(command.get("action"), settle)(db, trader, command, quote)
        status = 200
    except MarketError as error:
        db.execute("ROLLBACK TO settlement")
        result = {"message": str(error)}
        status = error.status
    db.execute("RELEASE settlement")
    db.execute("INSERT INTO requests VALUES(?,?,?,?,?)", (trader, key, payload, status, json.dumps(result)))
    return status, result


def dispatch(path, token, body, is_order, clock=time.time):
    require(isinstance(token, str) and re.fullmatch(r"[\w-]{43}", token, re.ASCII), 401,
            "Token pedagang tidak sah.")
    with transaction(path) as db:
        trader = db.execute("SELECT * FROM traders WHERE token_hash=?", (digest(token),)).fetchone()
        require(trader, 401, "Token pedagang tidak sah.")
        suspended = db.execute("SELECT 1 FROM suspensions WHERE trader=?", (trader["id"],)).fetchone()
        require(not suspended, 403, "Akun ditangguhkan. Hubungi pengelola pasar.")
        now = int(clock())
        quote = prices(advance(db, trader, now))
        contracts.run(db, quote["gameHour"], now)
        if is_order:
            return order(db, trader["id"], body, quote)
        return 200, board(db, trader["id"], quote)
