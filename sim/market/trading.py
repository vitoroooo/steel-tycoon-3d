"""Cash-constrained price discovery and inventory transfers."""
from market.budget import allowance


def bid(buyer, ref, c):
    prices = c['prices']
    base = prices['bandar'] if buyer.caste == 2 else prices['smelter']
    adjustment = prices['stock_response'] * (1 - buyer.stock / c['yard_capacity'])
    return ref * max(prices['minimum'], min(prices['maximum'], base + adjustment))


def settle(seller, quantity, price, cost, c, player=False):
    revenue = quantity * price * (1 - c['transaction_tax'] if player else 1)
    seller.cash += revenue
    seller.turnover += quantity * price
    profit = revenue - cost
    seller.profit += profit
    seller.quarter += profit


def sell(seller, quantity, cost, buyers, ref, c, flows, caps=None):
    remaining = quantity
    floor = ref * c['prices']['ai1' if seller.caste == 1 else 'ai2']
    offers = []
    for buyer in buyers:
        price = bid(buyer, ref, c)
        distance = abs(buyer.district - seller.district) * c['prices']['distance'] * ref
        net = price * (1 - c['transaction_tax']) - distance
        if net > floor:
            offers.append((net, buyer.id, price, buyer, distance))
    for _, _, price, buyer, distance in sorted(offers, reverse=True):
        room = max(0, min(c['yard_capacity'] - buyer.stock, buyer.demand))
        amount = min(remaining, room, max(0, buyer.cash) / price)
        if amount <= 0:
            continue
        buyer.cash -= amount * price
        buyer.stock += amount
        buyer.stock_cost += amount * price
        buyer.demand -= amount
        settle(seller, amount, price, cost * amount / quantity, c, True)
        seller.earn(-amount * distance)
        flows['player'] += amount
        remaining -= amount
        if remaining < 1e-6:
            break
    if remaining:
        if seller.caste == 1 or remaining >= c['ai_batch']:
            amount = remaining if caps is None else allowance(
                caps, seller.caste, seller.district, min(remaining, seller.npc_remaining))
            if seller.caste == 2 and amount < c['ai_batch']:
                if caps is not None:
                    caps[seller.caste][seller.district] += amount
                amount = 0
            settle(seller, amount, floor, cost * amount / quantity, c)
            remaining -= amount
            if caps is not None:
                seller.npc_remaining -= amount
    return remaining, cost * remaining / quantity if quantity else 0


def ai_supply(bandars, ref, c, flows):
    limits = [c['ai_daily_district']] * c['districts']
    for p in bandars:
        price = bid(p, ref, c)
        if price < ref * c['prices']['ai_seller']:
            continue
        qty = max(0, min(limits[p.district], p.demand,
                        c['yard_capacity'] - p.stock, p.cash / price))
        p.cash -= qty * price
        p.stock += qty
        p.stock_cost += qty * price
        p.demand -= qty
        limits[p.district] -= qty
        flows['ai'] += qty


def ai_industrial(smelters, ref, c, flows):
    limits = [c['ai_industrial_daily']] * c['districts']
    price = ref * c['prices']['ai_industrial']
    for p in smelters:
        qty = max(0, min(limits[p.district], p.demand,
                        c['yard_capacity'] - p.stock, p.cash / price))
        p.cash -= qty * price
        p.stock += qty
        p.stock_cost += qty * price
        limits[p.district] -= qty
        flows['ai'] += qty
