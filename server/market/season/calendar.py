"""The season calendar (PRD 11.3): 90 real days in one city, the last two weeks being Pekan Tutup Buku."""
from ..clock import RULES, TIME

SEASON = RULES["seasonRealDays"] * 86400


def city(number):
    names = RULES["cities"]
    return names[(number - 1) % len(names)]


def today(db):
    opened, now = db.execute("SELECT opened_at,last_time FROM city").fetchone()
    number, into = divmod(max(0, now - opened), SEASON)
    day = into // 86400 + 1
    return {"season": number + 1, "city": city(number + 1), "seasonDay": day,
            "seasonDaysLeft": RULES["seasonRealDays"] - day + 1, "closing": day >= RULES["closingDay"],
            # the game hour at which this city closes: contracts must be finished by then
            "seasonEnd": (number + 1) * SEASON // TIME["realSecondsPerGameHour"]}
