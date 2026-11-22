"""The end of a season: every business is frozen into its owner's Grup Usaha and all start again at Kasta 1."""
from ..clock import RULES
from ..contracts import release
from .calendar import city, today


def close(db, number, reference):
    """Nothing is lost: lots and contracts end without a penalty, the value of each business is written into its
    group's history with its rank, and part of it becomes Poin Grup. Then the new city opens with fresh accounts."""
    for lot in db.execute("SELECT id,seller,kg FROM lots WHERE status='open'").fetchall():
        db.execute("UPDATE lots SET status='cancelled' WHERE id=?", (lot["id"],))
        db.execute("UPDATE traders SET stock=stock+? WHERE id=?", (lot["kg"], lot["seller"]))
    traders = [row["id"] for row in db.execute("SELECT id FROM traders")]
    for trader in traders:
        release(db, trader)
    worth = db.execute("SELECT id, cash + stock * ? AS value FROM traders ORDER BY value DESC, id",
                       (reference,)).fetchall()
    for rank, row in enumerate(worth, 1):
        points = row["value"] // RULES["valuePerGroupPoint"]
        db.execute("INSERT INTO legacies VALUES(?,?,?,?,?,?)",
                   (row["id"], number, city(number), row["value"], rank, points))
        db.execute("INSERT OR IGNORE INTO groups(trader) VALUES(?)", (row["id"],))
        db.execute("UPDATE groups SET points=points+? WHERE trader=?", (points, row["id"]))
    db.execute("UPDATE traders SET cash=?, stock=?, accrued_at=(SELECT last_time FROM city)",
               (RULES["initialCash"], RULES["initialStockKg"]))
    db.execute("DELETE FROM standing")
    db.execute("DELETE FROM blockades")


def roll(db, quote):
    """Closes every season that has run out since anyone last talked to the market, then tells today's date."""
    date = today(db)
    state = db.execute("SELECT number FROM season_state").fetchone()
    for number in range(state["number"] if state else date["season"], date["season"]):
        close(db, number, quote["reference"])
    db.execute("INSERT OR REPLACE INTO season_state VALUES(1,?)", (date["season"],))
    return date
