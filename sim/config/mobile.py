"""Project mobile prices into the 90-day city model. K3 remains a design scenario."""
import json
from math import hypot
from pathlib import Path

RESOURCES = Path(__file__).resolve().parents[2] / 'unity/Assets/_Project/Resources/Config'


def resource(name):
    return json.loads((RESOURCES / f'{name}.json').read_text())


def apply_mobile(c):
    city, yard, time = (resource(n) for n in ('City', 'Business', 'Time'))
    market, routes = resource('Economy'), resource('Routes')
    local = resource('LocalMarket')
    c['local_counters'] = [
        (local[name + 'Fraction'], local['freightPerKgMetre'] *
         hypot(city['homeX'] - local[name + 'X'], city['homeZ'] - local[name + 'Z']))
        for name in ('pasar', 'bengkel')]
    c['first_pile_kg'] = min(city['carryCapKg'], resource('Piles')['p0kg'])
    c['yard_buffer'] = 0
    hour_seconds = time['realSecondsPerGameHour']
    c.update(mobile=True, start_cash=city['startCash'], tutorial=city['firstSaleGrant'],
             rent=yard['rent'], rent_days=yard['leaseHours'] * hour_seconds / 86400,
             roof=yard['pave'] + yard['roof'], yard_capacity=yard['capacityKg'],
             batch=yard['minShipmentKg'], trailer_capacity=yard['minShipmentKg'],
             trailer_trip=yard['shipFee'], free_speed=routes['freeLoadFraction'],
             offline_hours=time['offlineCapRealSeconds'] / 3600,
             game_days=3600 / hour_seconds, transaction_tax=market['transactionTax'])
    upgrade = c['upgrade']
    upgrade['cash'][0] = yard['minimumCash']
    upgrade['rep'][0] = yard['minimumReputation']
    upgrade['permit'][0] = yard['permit']
    upgrade['permit_days'][0] = yard['permitHours'] / 24
    upgrade['package'][0] = yard['scale'] + yard['loader']
    c['prices'].update(ai1=market['aiBuyK1Fraction'], ai2=market['aiBuyK2Fraction'],
                       npc=market['npcSellK1Fraction'], ai_industrial=market['aiSellSmelterFraction'],
                       ai_seller=.86)  # SupplierHour compares against CounterRate for HMS1.
    c['ibn'].update(base=market['baseIbn'], reference=market['scrapReferenceFraction'],
                    period=market['cycleRealSeconds'] / 86400, noise=market['dailyNoiseFraction'],
                    amplitude=market['cycleAmplitude'], supply_limit=market['supplyLimit'],
                    supply_target=market['supplyTargetKg'], ticks=86400 // market['refreshRealSeconds'])
    cart, *motors = c['vehicles']
    cart.update(capacity=resource('Gerobak')['capacityKg'], hours=routes['cartSeconds'] / 3600)
    for vehicle in motors:
        name = vehicle['name']
        motor = resource(name.capitalize())
        vehicle.update(capacity=motor['capacityKg'], cost=motor['price'],
                       hours=motor['tripRealSeconds'] / 3600, trip_cost=routes[f'{name}Fuel'])
    return c
