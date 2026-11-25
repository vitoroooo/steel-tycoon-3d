from .clock import RULES, tax_on


class MarketError(Exception):
    def __init__(self, status, message):
        super().__init__(message)
        self.status = status


def require(condition, status, message):
    if not condition:
        raise MarketError(status, message)


def whole(payload, field, maximum):
    value = payload[field]
    require(type(value) is int and 0 < value <= maximum, 400, "Angka pesanan di luar batas.")
    return value


def check_price(quote, price):
    require(quote["minimum"] <= price <= quote["maximum"], 409, "Harga di luar batas acuan terkini.")


def settle(db, trader, command, quote):
    action = command.get("action")
    if action == "sell":
        require(set(command) == {"action", "kg", "price"}, 400, "Kolom pesanan jual keliru.")
        kg = whole(command, "kg", RULES["maxLotKg"])
        price = whole(command, "price", 2**31 - 1)
        check_price(quote, price)
        existing = db.execute("SELECT 1 FROM lots WHERE seller=? AND status='open'", (trader,)).fetchone()
        require(not existing, 409, "Batalkan atau selesaikan lot sebelumnya.")
        debit = db.execute("UPDATE traders SET stock=stock-? WHERE id=? AND stock>=?", (kg, trader, kg))
        require(debit.rowcount, 409, "Stok akun pasar kurang.")
        lot = db.execute("INSERT INTO lots(seller,kg,price,status) VALUES(?,?,?,'open')",
                         (trader, kg, price))
        return {"lot": lot.lastrowid, "message": "Lot dipasang. Stok dititipkan di pasar."}

    require(action in ("buy", "cancel") and set(command) == {"action", "lot"}, 400, "Perintah keliru.")
    number = whole(command, "lot", 2**63 - 1)
    lot = db.execute("SELECT * FROM lots WHERE id=? AND status='open'", (number,)).fetchone()
    require(lot, 409, "Lot sudah terjual, dibatalkan, atau tidak tersedia.")
    seller, kg = lot["seller"], lot["kg"]
    if action == "cancel":
        require(seller == trader, 403, "Hanya penjual dapat membatalkan lot.")
        db.execute("UPDATE lots SET status='cancelled' WHERE id=?", (number,))
        db.execute("UPDATE traders SET stock=stock+? WHERE id=?", (kg, seller))
        return {"lot": number, "message": "Lot dibatalkan. Stok telah kembali."}

    require(seller != trader, 409, "Pilih lot milik pedagang lain.")
    check_price(quote, lot["price"])
    gross = kg * lot["price"]
    tax = tax_on(gross)
    debit = db.execute("UPDATE traders SET cash=cash-?,stock=stock+? WHERE id=? AND cash>=?",
                       (gross, kg, trader, gross))
    require(debit.rowcount, 409, "Kas akun pasar kurang.")
    db.execute("UPDATE traders SET cash=cash+? WHERE id=?", (gross - tax, seller))
    db.execute("UPDATE lots SET status='sold' WHERE id=?", (number,))
    db.execute("INSERT INTO trades VALUES(?,?,?,?,?)", (number, trader, gross, tax, quote["gameHour"]))
    return {"lot": number, "gross": gross, "tax": tax, "message": "Pembelian lunas. Stok telah diterima."}
