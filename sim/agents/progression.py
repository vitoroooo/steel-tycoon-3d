"""Purchases and permission/cash/reputation gates."""


def eligible(p, c):
    if p.caste >= 3:
        return False
    i = p.caste - 1
    return (p.cash >= c['upgrade']['cash'][i]
            and p.reputation >= c['upgrade']['rep'][i])


def progress(p, day, c):
    if p.caste == 1 and p.vehicle < len(c['vehicles']) - 1:
        v = c['vehicles'][p.vehicle + 1]
        if p.cash >= v['cost']:
            p.cash -= v['cost']
            p.assets += v['cost']
            p.vehicle += 1
            p.mark(v['name'], day)
    if (p.caste == 2 and not p.roof
            and p.cash >= c['roof'] + c['working_reserve']):
        p.cash -= c['roof']
        p.assets += c['roof']
        p.roof = True
        p.mark('cor_hanggar', day)
    if eligible(p, c):
        i = p.caste - 1
        if p.permit != p.caste:
            if p.cash < c['upgrade']['cash'][i] + c['upgrade']['permit'][i]:
                return
            p.cash -= c['upgrade']['permit'][i]
            p.permit = p.caste
            p.ready = day + c['upgrade']['permit_days'][i] / c['game_days']
        if day >= p.ready:
            p.cash -= c['upgrade']['package'][i]
            p.assets += c['upgrade']['package'][i]
            p.caste += 1
            p.wait_since = day
            p.mark(f'kasta{p.caste}', day)
    if (p.caste == 3 and 'b2g' not in p.milestones
            and day - p.born + 1 >= p.milestones['kasta3'] + c['b2g_delay']
            and not getattr(p, 'b2g_ready', False) and p.cash >= c['b2g_permit']):
        p.cash -= c['b2g_permit']
        p.b2g_ready = True
