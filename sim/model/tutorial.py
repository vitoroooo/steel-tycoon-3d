"""Settle the initial accessible pile before allocating the rest of the first day."""
from agents.mobile import credit_sale
from agents.progression import progress
from market.trading import sell


def first_trip(p, buyers, sources, caps, flows, ref, day, c):
    if not c.get('mobile') or p.born != day:
        return 0
    kg = min(c['first_pile_kg'], sources[p.district])
    if kg:
        sources[p.district] -= kg
        p.stock, p.stock_cost = sell(p, kg, 0, buyers, ref, c, flows, caps)
        credit_sale(p, kg - p.stock, day, c)
        progress(p, day, c)
    return 5 / 60  # One five-minute onboarding session.
