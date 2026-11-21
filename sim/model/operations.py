"""Daily activity from session queues, inventories and production capacities."""
from market.trading import sell, ai_supply, ai_industrial, settle


def hours(p, c):
    return min(c['real_hours'], p.sessions * c['offline_hours'])


def operate(players, ref, c, flows, day, waits):
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
        qty = trips * v['capacity'] * (1 - free + free * c['free_speed'])
        cost = qty * ref * c['prices']['npc'] * (1 - free) + trips * v['trip_cost']
        if cost > p.cash:
            scale = max(0, p.cash / cost)
            qty, cost = qty * scale, cost * scale
        p.cash -= cost
        if qty:
            sell(p, qty, cost, bandars, ref, c, flows)
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
        p.earn(-transport)
        left, left_cost = sell(p, qty, cost, smelters, ref, c, flows)
        p.stock, p.stock_cost = left / (1 - c['dirt']), left_cost
        p.reputation += c['rep_per_contract']
    ai_industrial(smelters, ref, c, flows)
    for p in smelters:
        qty = min(p.stock, c['melt_daily'] * c['game_days'] * hours(p, c) / c['real_hours'])
        if qty <= 0:
            continue
        cost = p.stock_cost * qty / p.stock
        output = qty * (1 - c['melt_loss'])
        p.earn(-output * c['melt_opex'])
        settle(p, output, ref / c['ibn']['reference'], cost, c)
        p.stock -= qty
        p.stock_cost -= cost
        flows['ingot'] += output
        if getattr(p, 'b2g_ready', False):
            p.mark('b2g', day)
