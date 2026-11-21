"""Metrics include censoring and both price and monetary inflation."""
from statistics import median


def med(values):
    values = list(values)
    return median(values) if values else None


def summarize(result):
    players, daily = result['players'], result['daily']
    active = [p for p in players if p.kind == 'aktif' and p.born == 0]
    milestones = {}
    for name in ('triseda', 'pikap', 'kasta2', 'cor_hanggar', 'kasta3', 'b2g'):
        hits = [p.milestones[name] for p in active if name in p.milestones]
        milestones[name] = dict(median=med(hits), reached=len(hits), cohort=len(active))
    weekly = []
    for end in range(6, len(daily), 7):
        start = max(0, end - 7)
        a, b = daily[start], daily[end]
        weekly.append(dict(day=b['day'], money=b['money'],
                           monetary=b['money'] / a['money'] - 1 if a['money'] else 0,
                           price=b['ibn'] / a['ibn'] - 1))
    caste_profit = {k: med(r['profit'] for r in result['profits'] if r['caste'] == k)
                    for k in (1, 2, 3)}
    shares = [d['ai_share'] for d in daily[:60] if d['ai_share'] is not None]
    k3 = milestones['kasta3']['median']
    checks = dict(profit=all(v is not None and v > 0 for v in caste_profit.values()),
                  ai=len(shares) == min(60, len(daily)) and bool(shares) and max(shares) < 0.5,
                  inflation=all(w['monetary'] < 0.05 for w in weekly),
                  progression=k3 is not None and 45 <= k3 <= 55)
    return dict(milestones=milestones, weekly=weekly, profit=caste_profit,
                wait=med(result['waits']), max_ai=max(shares) if shares else None,
                no_demand=sum(d['ai_share'] is None for d in daily[:60]), checks=checks)
