import json
import math
import random
from pathlib import Path

HERE = Path(__file__).resolve().parent
CONFIG = HERE.parents[1] / "unity/Assets/_Project/Resources/Config"
ECONOMY = json.loads((CONFIG / "Economy.json").read_text())
TIME = json.loads((CONFIG / "Time.json").read_text())
RULES = json.loads((HERE / "rules.json").read_text())


def advance(db, trader, now):
    db.execute("UPDATE city SET last_time = MAX(last_time, ?)", (now,))
    opened, now = db.execute("SELECT opened_at,last_time FROM city").fetchone()
    tick = TIME["realSecondsPerGameHour"]
    elapsed = max(0, now - trader["accrued_at"])
    # the legacy "antrean offline +2 jam" (PRD 11.3) lengthens the cap in the season it was picked for
    season = (now - opened) // (RULES["seasonRealDays"] * 86400) + 1
    longer = db.execute("SELECT 1 FROM groups WHERE trader=? AND picked=? AND instr(','||picks||',', ',antrean,')",
                        (trader["id"], season)).fetchone()
    capped = min(elapsed, TIME["offlineCapRealSeconds"] + (RULES["legacyQueueHours"] * tick if longer else 0))
    hours = capped // tick
    # an account whose career runs on the career server collects through that career, not through this free route
    if hours and not db.execute("SELECT 1 FROM wallets WHERE trader=?", (trader["id"],)).fetchone():
        # Move past discarded time as well, so repeated reads cannot farm the cap.
        db.execute("UPDATE traders SET accrued_at=?, stock=stock+? WHERE id=?",
                   (now - capped % tick, hours * RULES["collectionKgPerGameHour"], trader["id"]))
    return (now - opened) // tick


def prices(hour):
    seconds = hour * TIME["realSecondsPerGameHour"]
    day = seconds // 86400
    noise = random.Random(ECONOMY["noiseSeed"] + day).uniform(-1, 1)
    cycle = math.sin(2 * math.pi * seconds / ECONOMY["cycleRealSeconds"])
    # The local HMS 1 market has no ingot deliveries. PRD 7.1 caps this supply effect at +5%.
    ibn = round(ECONOMY["baseIbn"] * (1 + cycle * ECONOMY["cycleAmplitude"])
                * (1 + noise * ECONOMY["dailyNoiseFraction"]) * (1 + ECONOMY["supplyLimit"]))
    reference = round(ibn * ECONOMY["scrapReferenceFraction"])
    return {"gameHour": hour, "ibn": ibn, "reference": reference,
            "minimum": (reference * RULES["minReferencePercent"] + 99) // 100,
            "maximum": reference * RULES["maxReferencePercent"] // 100}


def tax_on(gross):
    percent = round(100 * ECONOMY["transactionTax"])
    return (gross * percent + 50) // 100
