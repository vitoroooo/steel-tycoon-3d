"""The daily deliveries of running contracts, settled whenever anyone talks to the market."""
from ..clock import RULES, advance, tax_on
from .terms import DAY, close, lock


def deliver(db, contract):
    """One game day's delivery: stock to the buyer, the locked money to the seller, and the next day's money locked.
    A seller without the stock, or a buyer without the next day's money, breaks the contract and pays the penalty."""
    kg, number = contract["kg"], contract["id"]
    moved = db.execute("UPDATE traders SET stock=stock-? WHERE id=? AND stock>=?", (kg, contract["seller"], kg))
    if not moved.rowcount:
        close(db, contract, contract["seller"], False)
        return
    gross = contract["locked"]
    db.execute("UPDATE traders SET stock=stock+? WHERE id=?", (kg, contract["buyer"]))
    db.execute("UPDATE traders SET cash=cash+? WHERE id=?", (gross - tax_on(gross), contract["seller"]))
    done = contract["delivered"] + 1 == contract["days"]
    db.execute("UPDATE contracts SET delivered=delivered+1, locked=0, due=due+?, status=? WHERE id=?",
               (DAY, "done" if done else "active", number))
    contract = db.execute("SELECT * FROM contracts WHERE id=?", (number,)).fetchone()
    if not done and not lock(db, contract):
        close(db, contract, contract["buyer"], False)


def run(db, hour, now):
    """Settles every delivery that has come due, oldest contract first."""
    while True:
        # a blockaded route moves nothing: those deliveries wait until it is open again
        contract = db.execute("""SELECT * FROM contracts c WHERE status='active' AND due<=? AND NOT EXISTS
                                 (SELECT 1 FROM blockades b WHERE b.target IN (c.seller, c.buyer) AND b.until>?)
                                 ORDER BY due,id""", (hour, hour)).fetchone()
        if not contract:
            return
        # the seller's own collecting up to now counts, even if he has not looked at the market since
        advance(db, db.execute("SELECT * FROM traders WHERE id=?", (contract["seller"],)).fetchone(), now)
        deliver(db, contract)


def listing(db, trader):
    rows = db.execute("""SELECT c.id, s.name AS seller, b.name AS buyer, c.kg, c.price, c.days, c.delivered,
                                c.locked, c.due, c.penalty, c.status, c.seller=? AS selling
                         FROM contracts c JOIN traders s ON s.id=c.seller JOIN traders b ON b.id=c.buyer
                         WHERE ? IN (c.seller, c.buyer) ORDER BY c.id DESC LIMIT ?""",
                      (trader, trader, RULES["contractsShown"]))
    return [dict(row) for row in rows]


def partners(db, trader):
    """The other traders in the city a contract can be offered to."""
    rows = db.execute("""SELECT name FROM traders WHERE id!=? AND id NOT IN (SELECT trader FROM suspensions)
                         ORDER BY name LIMIT ?""", (trader, RULES["partnersShown"]))
    return [row["name"] for row in rows]
