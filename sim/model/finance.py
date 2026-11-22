"""Recurring expenses, accrual-based quarterly tax, debt and seizure."""


def close_day(p, day, c):
    w = c['workers']
    if p.caste == 1:
        wages = w['driver'] if p.vehicle == 2 else 0
    elif p.caste == 2:
        wages = w['crew'] * w['kuli'] + w['driver'] + w['operator']
        if not c.get('mobile'):
            p.earn(-c['rent'] / c['rent_days'])
    else:
        wages = w['eaf_count'] * w['eaf'] + w['operator'] + w['driver']
    if c.get('mobile') and day < p.lease_until:
        # Cash was paid upfront. Recognize the expense over the term, even after promotion.
        expense = c['rent'] / c['rent_days']
        p.profit -= expense
        p.quarter -= expense
    duty = min(1, getattr(p, 'workload', 1)) if c['variable_staff'] else 1
    p.earn(-wages * c['game_days'] * duty)
    p.earn(-p.assets * c['maintenance'])
    p.earn(-p.turnover * c['sales_maintenance'][p.caste - 1])
    if (day + 1) % c['tax_days'] == 0:
        p.earn(-max(0, p.quarter) * c['tax'][p.caste - 1])
        p.quarter = 0
    if p.cash < 0:
        p.earn(p.cash * ((1 + c['debt_rate']) ** c['debt_periods'] - 1))
        if -p.cash > p.assets * c['seizure_ratio'] and p.assets:
            seized = min(p.assets, -p.cash)
            p.assets -= seized
            p.cash += seized
            if p.assets == 0:
                p.vehicle = 0
                p.roof = False
                p.permit = 0
                if p.cash < 0 and p.caste > 1:
                    p.caste -= 1
                    p.stock = p.stock_cost = 0
                    p.cash = c['start_cash']
