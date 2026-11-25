"""Associations, their chat, and route blockades (PRD 6.3 and 8.1)."""
from .blockade import blockade, blocked, escort, gate, promote, standing
from .chat import recent, report, say
from .members import directory, found, join, leave, mine, roster

ACTIONS = {"found": found, "join": join, "leave": leave, "say": say, "report": report,
           "promote": promote, "blockade": blockade, "escort": escort}


def summary(db, trader, hour):
    """What the board tells a trader about his caste, his association and a blockade of his route."""
    own, block = mine(db, trader), blocked(db, trader, hour)
    rivals = db.execute("""SELECT t.name FROM standing s JOIN traders t ON t.id=s.trader
                           LEFT JOIN members m ON m.trader=t.id
                           WHERE s.caste=3 AND t.id!=? AND (m.association IS NULL OR m.association!=?)
                           ORDER BY t.name""", (trader, own["id"] if own else 0))
    return {"caste": standing(db, trader)[0],
            "blockedUntil": block["until"] if block else 0, "blockedBy": block["name"] if block else "",
            "association": {"name": own["name"] if own else "",
                            "members": roster(db, own["id"]) if own else [],
                            "messages": recent(db, own["id"]) if own else []},
            "associations": directory(db), "rivals": [row["name"] for row in rivals]}


__all__ = ["ACTIONS", "gate", "summary"]
