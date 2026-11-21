"""Seeded daily agent scheduler; hourly IBN ticks use yesterday's output."""
import random
from agents.player import Player
from agents.progression import progress
from agents.population import initial_roles, seed_role
from model.economics import ibn, survival
from model.operations import operate
from model.finance import close_day


def run(c):
    rng = random.Random(c['seed'])
    players, daily, profits, waits = [], [], [], []
    supply = 0
    roles = initial_roles(c)
    for day in range(c['days']):
        count = c['agents'] if day == 0 else round(c['agents'] * c['arrival_rate'])
        for _ in range(count):
            kinds = list(c['types'])
            position = (_ + 0.5) / count
            cumulative = 0
            for kind in kinds:
                cumulative += c['types'][kind][0]
                if position < cumulative:
                    break
            p = Player(len(players), kind, c['types'][kind][1],
                       rng.randrange(c['districts']), day,
                       c['start_cash'] + c['tutorial'], reputation=c['rep_start'])
            if day == 0:
                seed_role(p, roles[_], c)
            players.append(p)
        live = []
        for p in players:
            age = day - p.born
            if p.active and age:
                chance = survival(age, c['retention']) / survival(age - 1, c['retention'])
                p.active = rng.random() < chance
            if p.active:
                live.append(p)
        rng.shuffle(live)
        for p in live:
            progress(p, day, c)
        before = {p.id: (p.caste, p.profit) for p in live}
        noise = rng.uniform(1 - c['ibn']['noise'], 1 + c['ibn']['noise'])
        prices = [ibn(day + tick / c['ibn']['ticks'], noise, supply, c['ibn'])
                  for tick in range(c['ibn']['ticks'])]
        index = sum(prices) / len(prices)
        flows = dict(ai=0, player=0, ingot=0)
        operate(live, index * c['ibn']['reference'], c, flows, day, waits)
        for p in live:
            close_day(p, day, c)
            caste, old = before[p.id]
            profits.append(dict(day=day + 1, id=p.id, caste=caste,
                                profit=p.profit - old))
        total = flows['ai'] + flows['player']
        daily.append(dict(day=day + 1, **{f'kasta{k}': sum(p.caste == k for p in live)
                                        for k in (1, 2, 3)}, ibn=index,
                          ai_share=flows['ai'] / total if total else None,
                          **flows, money=sum(max(0, p.cash) for p in players),
                          debt=sum(max(0, -p.cash) for p in players)))
        supply = flows['ingot']
    return dict(players=players, daily=daily, profits=profits, waits=waits)
