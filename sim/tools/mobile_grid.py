"""Only experimental policy knobs are writable in this fixed-target experiment."""
from copy import deepcopy
from itertools import product


def candidates(base, plan):
    yield 'control', {}, base
    axes = ('bandar_bid', 'late_k1_cap_kg', 'late_free_trips', 'extra_liquidity', 'service_rates')
    for index, values in enumerate(product(*(plan[key] for key in axes)), 1):
        bid, limit, free, buffer, service = values
        c = deepcopy(base)
        c['prices']['bandar'] = bid
        c['npc_caps_late'][0] = limit
        c['free_late'] = dict.fromkeys(c['types'], free)
        c['yard_buffer'] = buffer
        c['sales_maintenance'] = service
        yield f'policy-{index:02d}', dict(zip(axes, values)), c


def rank(rows, targets):
    def inside(value, bounds):
        return value is not None and bounds[0] <= value <= bounds[1]
    detailed = 0
    for r in rows:
        detailed += sum(value is not None and value > 0 for value in r['profit'].values())
        detailed += inside(r['mix'][2], targets['k2_share']) + inside(r['mix'][3], targets['k3_share'])
        detailed += inside(r['k2']['median'], targets['active_k2'])
        detailed += inside(r['casual']['median'], targets['casual_k2'])
    return (sum(all(r['checks'].values()) for r in rows),
            sum(sum(r['checks'].values()) for r in rows), detailed)
