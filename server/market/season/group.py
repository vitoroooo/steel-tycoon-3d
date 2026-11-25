"""Grup Usaha (PRD 11.3): what a trader keeps across seasons, and the three legacies he starts a new city with."""
from ..clock import RULES
from ..orders import require

LEGACIES = {item["id"]: item for item in RULES["legacies"]}


def level(points):
    return 1 + sum(points >= need for need in RULES["groupLevelPoints"])


def row(db, trader):
    found = db.execute("SELECT points, picks, picked FROM groups WHERE trader=?", (trader,)).fetchone()
    return found or {"points": 0, "picks": "", "picked": 0}


def history(db, trader):
    rows = db.execute("SELECT season, city, value, rank, points FROM legacies WHERE trader=? ORDER BY season DESC",
                      (trader,))
    return [dict(item) for item in rows]


def bekal(db, trader, command, quote):
    require(set(command) == {"action", "picks"} and type(command["picks"]) is str, 400, "Kolom bekal keliru.")
    group, chosen = row(db, trader), command["picks"].split(",")
    require(history(db, trader), 409, "Bekal warisan terbuka setelah Anda menutup satu season.")
    require(group["picked"] != quote["season"], 409, "Bekal season ini sudah dipilih.")
    require(len(set(chosen)) == len(chosen) <= RULES["legacyPicks"], 400, "Pilih paling banyak 3 bekal berbeda.")
    for name in chosen:
        require(name in LEGACIES, 400, "Bekal tidak dikenal.")
        require(LEGACIES[name]["level"] <= level(group["points"]), 409, "Bekal itu belum terbuka di level grup Anda.")
    db.execute("UPDATE groups SET picks=?, picked=? WHERE trader=?", (",".join(chosen), quote["season"], trader))
    return {"message": "Bekal warisan dipilih untuk season ini."}


def summary(db, trader, quote):
    group, past = row(db, trader), history(db, trader)
    picked = group["picked"] == quote["season"]
    return {"group": {"points": group["points"], "level": level(group["points"]), "history": past,
                      "picks": group["picks"].split(",") if picked and group["picks"] else [],
                      "choose": bool(past) and not picked, "legacies": RULES["legacies"]}}
