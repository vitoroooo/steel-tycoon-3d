"""Transfer, licence and prepaid lease rules of the mobile career."""
import math


def credit_sale(p, kg, day, c):
    if kg <= 0:
        return
    p.sold_kg += kg
    p.reputation = min(100, 1 + math.floor(p.sold_kg / 100))
    if 'first_sale' not in p.milestones:
        p.mark('first_sale', day)
        p.cash += c['tutorial']  # Claim at dealer after selling. Transfer, not trading profit.


def renew(p, day, c):
    if day >= p.lease_until and p.cash >= c['rent']:
        p.cash -= c['rent']
        p.lease_until = day + c['rent_days']


def open_yard(p, day, c):
    u = c['upgrade']
    if p.goal < 2 or p.reputation < u['rep'][0]:
        return
    if p.permit == 0:
        # This agent prioritizes a pickup, then files a complete licence application.
        if p.vehicle != 2 or p.cash < u['permit'][0]:
            return
        p.cash -= u['permit'][0]
        p.permit = 1
        p.ready = day + u['permit_days'][0] / c['game_days']
    if day < p.ready or p.cash < c['rent'] + max(u['cash'][0] + c['yard_buffer'], u['package'][0]):
        return
    renew(p, day, c)
    p.cash -= u['package'][0]
    p.assets += u['package'][0]
    p.caste = 2
    p.wait_since = day
    p.mark('kasta2', day)
