"""Daily activity from session queues, inventories and production capacities."""
from collections import Counter
from market.trading import sell, ai_supply, ai_industrial, settle
from market.budget import budgets, allowance


def hours(p, c):
    return min(c['real_hours'], p.sessions * c['offline_hours'])


def operate(players, ref, c, flows, day, waits):
    caps = budgets(c, day)
    sources = [c['source_cap']] * c['districts']
    counts = Counter((p.caste, p.district) for p in players)
    for p in players:
        peers = counts[p.caste, p.district]
        p.npc_remaining = caps[p.caste][p.district] / peers
        p.workload = 0
        p.turnover = 0
    bandars = [p for p in players if p.caste == 2]
    smelters = [p for p in players if p.caste == 3]
    for p in bandars + smelters:
        capacity = c['bandar_daily'] if p.caste == 2 else c['melt_daily'] * c['game_days']
        p.demand = capacity * min(1, hours(p, c) / c['real_hours'])
    for p in players:
        if p.caste != 1:
            continue
        v = c['vehicles'][p.vehicle]
        trips = hours(p, c) * c['queue_efficiency'] / v['hours']
        free = c['free_fraction']
        if day >= c['faucet_change_day']:
            free = c['free_late'][p.kind]
        qty = trips * v['capacity'] * (1 - free + free * c['free_speed'])
        cost = qty * ref * c['prices']['npc'] * (1 - free) + trips * v['trip_cost']
        scale = min(1, sources[p.district] / qty if qty else 0,
                    max(0, c['yard_capacity'] - p.stock) / qty if qty else 0,
                    max(0, p.cash) / cost if cost else 1)
        qty, cost = min(qty * scale, sources[p.district]), cost * scale
        p.workload = scale
        sources[p.district] -= qty
        p.cash -= cost
        qty, cost = qty + p.stock, cost + p.stock_cost
        if qty:
            p.stock, p.stock_cost = sell(p, qty, cost, bandars, ref, c, flows, caps)
            p.reputation += c['rep_per_sale'] * p.sessions
    ai_supply(bandars, ref, c, flows)
    for p in bandars:
        if p.stock < c['batch']:
            continue
        waits.append(day - p.wait_since + 1)
        p.wait_since = day + 1
        raw, cost = p.stock, p.stock_cost
        qty = raw * (1 - c['dirt'])
        transport = c['trailer_trip'] * raw / c['trailer_capacity']
        left, left_cost = sell(p, qty, cost, smelters, ref, c, flows, caps)
        sold_fraction = (qty - left) / qty
        p.earn(-transport * sold_fraction)
        p.workload = raw * sold_fraction / c['bandar_daily']
        p.stock, p.stock_cost = left / (1 - c['dirt']), left_cost
        p.reputation += c['rep_per_contract']
    ai_industrial(smelters, ref, c, flows)
    for p in smelters:
        qty = min(p.stock, c['melt_daily'] * c['game_days'] * hours(p, c) / c['real_hours'])
        output = allowance(caps, 3, p.district,
                           min(p.npc_remaining, qty * (1 - c['melt_loss'])))
        qty = min(qty, output / (1 - c['melt_loss']))
        if qty <= 0:
            continue
        cost = min(p.stock_cost, p.stock_cost * qty / p.stock)
        p.earn(-output * c['melt_opex'])
        p.workload = qty / (c['melt_daily'] * c['game_days'])
        settle(p, output, ref / c['ibn']['reference'], cost, c)
        p.stock -= qty
        p.stock_cost -= cost
        flows['ingot'] += output
        if getattr(p, 'b2g_ready', False):
            p.mark('b2g', day)
