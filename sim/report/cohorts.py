"""Right-censored milestone medians of the initial day-90 survivor cohort."""
from math import isfinite
from statistics import median


def milestone(players, name):
    ages = [p.milestones.get(name, float('inf')) for p in players]
    value = median(ages) if ages else float('inf')
    return dict(median=value if isfinite(value) else None,
                reached=sum(isfinite(age) for age in ages), cohort=len(ages))


def survivors(players, kind):
    return [p for p in players if p.kind == kind and p.born == 0 and p.active]
