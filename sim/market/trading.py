"""Cash-constrained price discovery and inventory transfers."""
from market.budget import allowance
from market.quotes import bid, npc_quote


def settle(seller, quantity, price, cost, c, player=False):
    revenue = quantity * price * (1 - c['transaction_tax'] if player else 1)
    seller.cash += revenue
    seller.turnover += quantity * price
    profit = revenue - cost
    seller.profit += profit
    seller.quarter += profit


def sell(seller, quantity, cost, buyers, ref, c, flows, caps=None):
    remaining = quantity
    fallback, freight = npc_quote(seller.caste, ref, c)
    floor = fallback - freight
    lot = c['batch'] * (1 - c['dirt']) if c.get('mobile') and seller.caste == 2 else 0
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
        if lot:
            amount = int(amount / lot) * lot
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
            shipped = int(amount / lot) * lot if lot else amount
            if seller.caste == 2 and shipped < c['ai_batch']:
                shipped = 0
            if shipped != amount:
                if caps is not None:
                    caps[seller.caste][seller.district] += amount - shipped
                amount = shipped
            settle(seller, amount, fallback, cost * amount / quantity, c)
            if freight:
                seller.earn(-amount * freight)
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
