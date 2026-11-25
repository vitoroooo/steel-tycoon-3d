import time

from .storage import transaction


def moderate(path, name, suspend):
    with transaction(path) as db:
        account = db.execute("SELECT id FROM traders WHERE name=?", (name,)).fetchone()
        if not account:
            raise ValueError("Pedagang belum terdaftar.")
        trader = account["id"]
        if suspend:
            changed = db.execute("INSERT OR IGNORE INTO suspensions VALUES(?)", (trader,))
            lot = db.execute("SELECT id,kg FROM lots WHERE status='open' AND seller=?", (trader,)).fetchone()
            if lot:
                db.execute("UPDATE lots SET status='cancelled' WHERE id=?", (lot["id"],))
                db.execute("UPDATE traders SET stock=stock+? WHERE id=?", (lot["kg"], trader))
        else:
            changed = db.execute("DELETE FROM suspensions WHERE trader=?", (trader,))
        if changed.rowcount:
            db.execute("INSERT INTO moderator_events(trader,operation,recorded_at) VALUES(?,?,?)",
                       (trader, "suspend" if suspend else "restore", int(time.time())))
