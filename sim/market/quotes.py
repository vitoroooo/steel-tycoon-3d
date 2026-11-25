"""Inventory-sensitive player bids and the best NPC offer for a seller."""


def bid(buyer, ref, c):
    prices = c['prices']
    base = prices['bandar'] if buyer.caste == 2 else prices['smelter']
    adjustment = prices['stock_response'] * (1 - buyer.stock / c['yard_capacity'])
    return ref * max(prices['minimum'], min(prices['maximum'], base + adjustment))


def npc_quote(caste, ref, c):
    if caste == 2:
        return ref * c['prices']['ai2'], 0
    offers = [(ref * c['prices']['ai1'], 0)]
    offers.extend((ref * fraction, fee) for fraction, fee in c.get('local_counters', []))
    return max(offers, key=lambda offer: offer[0] - offer[1])
