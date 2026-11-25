"""Pure economic calculations; rupiah and kilograms throughout."""
import math


def ibn(day, random_factor, supply, c):
    effect = max(-c['supply_limit'], min(c['supply_limit'],
                 (1 - supply / c['supply_target']) * c['supply_limit']))
    cycle = 1 + c['amplitude'] * math.sin(2 * math.pi * day / c['period'])
    return c['base'] * cycle * random_factor * (1 + effect)


def margin(buy, sell, cost=0, loss=0, tax=0):
    if not 0 <= loss < 1:
        raise ValueError('loss must be in [0, 1)')
    return sell * (1 - tax) - buy / (1 - loss) - cost


def survival(age, points):
    for (left, a), (right, b) in zip(points, points[1:]):
        if age <= right:
            return a * (b / a) ** ((age - left) / (right - left))
    return points[-1][1]


def contraband_ev(multiplier, heat, c):
    risk = min(1, c['base_risk'] + heat * c['heat_risk'])
    legal = c['sale'] - c['cost']
    revenue = c['cost'] + legal * multiplier
    return (1 - risk) * (revenue - c['cost']) - risk * (
        c['cost'] + c['fine'] * revenue)
