"""Finite district source and NPC purchase budgets, reset once daily."""


def budgets(c, day):
    caps = c['npc_caps_early' if day < c['faucet_change_day'] else 'npc_caps_late']
    if len(caps) != 3 or any(cap < 0 for cap in caps):
        raise ValueError('three nonnegative NPC purchase caps required')
    return {k: [cap] * c['districts'] for k, cap in enumerate(caps, 1)}


def allowance(caps, caste, district, requested):
    if requested < 0 or caps[caste][district] < 0:
        raise ValueError(f'negative quantity/budget: {requested}, {caps[caste][district]}')
    quantity = min(requested, caps[caste][district])
    caps[caste][district] -= quantity
    return quantity
