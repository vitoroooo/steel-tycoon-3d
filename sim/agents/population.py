"""Initial role stress fixture, separate from the all-Kasta-1 season."""
import random


def initial_roles(c):
    roles = []
    for caste, fraction in enumerate(c['initial_mix'], 1):
        roles.extend([caste] * round(c['agents'] * fraction))
    random.Random(c['seed']).shuffle(roles)
    return roles


def seed_role(p, caste, c):
    if caste == 1:
        return
    p.caste = caste
    p.vehicle = len(c['vehicles']) - 1
    p.cash = c['initial_tier_cash'][caste - 1]
    p.assets = c['upgrade']['package'][caste - 2]
    p.reputation = c['upgrade']['rep'][caste - 2]
    p.permit = caste - 1
    p.mark(f'kasta{caste}', p.born)
