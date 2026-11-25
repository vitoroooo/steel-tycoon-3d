"""Metrics include censoring and both price and monetary inflation."""
from statistics import median
from report.cohorts import milestone, survivors


def med(values):
    values = list(values)
    return median(values) if values else None


def summarize(result):
    players, daily = result['players'], result['daily']
    active = survivors(players, 'aktif')
    t = result['config']['targets']
    milestones = {}
    for name in ('triseda', 'pikap', 'kasta2', 'cor_hanggar', 'kasta3', 'b2g'):
        milestones[name] = milestone(active, name)
    casual = milestone(survivors(players, 'santai'), 'kasta2')
    weekly = []
    for end in range(6, len(daily), 7):
        start = max(0, end - 7)
        a, b = daily[start], daily[end]
        money = result['opening_money'] if end == 6 else a['money']
        weekly.append(dict(day=b['day'], money=b['money'],
                           monetary=b['money'] / money - 1 if money else 0,
                           price=b['ibn'] / a['ibn'] - 1))
    caste_profit = {k: med(r['profit'] for r in result['profits'] if r['caste'] == k)
                    for k in (1, 2, 3)}
    shares = [d['ai_share'] for d in daily[:t['ai_until']] if d['ai_share'] is not None]
    last = daily[-1] if daily else {f'kasta{k}': 0 for k in (1, 2, 3)}
    total = sum(last[f'kasta{k}'] for k in (1, 2, 3))
    mix = {k: last[f'kasta{k}'] / total if total else 0 for k in (1, 2, 3)}
    def inside(value, bounds):
        return value is not None and bounds[0] <= value <= bounds[1]
    progression = (inside(mix[2], t['k2_share']) and inside(mix[3], t['k3_share'])
                   and inside(milestones['kasta2']['median'], t['active_k2'])
                   and inside(casual['median'], t['casual_k2']))
    tested = [w for w in weekly if w['day'] > t['inflation_after']]
    checks = dict(profit=all(v is not None and v > 0 for v in caste_profit.values()),
                  ai=len(daily) >= t['ai_until'] and bool(shares) and max(shares) < t['ai_limit'],
                  inflation=bool(tested) and all(w['money'] > 0 and w['monetary'] < t['inflation']
                                                for w in tested), progression=progression)
    return dict(milestones=milestones, weekly=weekly, profit=caste_profit,
                wait=med(result['waits']), max_ai=max(shares) if shares else None,
                no_demand=sum(d['ai_share'] is None for d in daily[:t['ai_until']]), checks=checks,
                casual=casual, mix=mix)
