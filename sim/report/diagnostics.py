"""Analytical stress tests are separate from simulated observations."""
from model.economics import contraband_ev, margin


def diagnostics(c):
    ref = c['ibn']['base'] * c['ibn']['reference']
    spread = margin(ref * c['prices']['npc'], ref * c['prices']['ai1'],
                    c['vehicles'][2]['trip_cost'] / c['vehicles'][2]['capacity'])
    x, y, z = c['ratio']
    share = z / (x * y + y + z)
    n3 = c['agents'] * c['target_k3']
    required = n3 * y * (x + 1)
    shortage = max(0, 1 - (c['agents'] - n3) / required)
    cb = c['contraband']
    legal = cb['sale'] - cb['cost']
    ev = {f'{m:.4f}x_heat{h}': contraband_ev(m, h, cb) / legal
          for m in cb['multipliers'] for h in (0, 15, 50, 65)}
    debt = {str(days): (1 + c['debt_rate']) ** (days * c['game_days'])
            for days in (1, 7, 30)}
    return dict(arbitrage_per_kg=spread, pyramid_k3_share=share,
                target_shortage=shortage, contraband_ev_legal_ratio=ev,
                debt_multipliers=debt)
