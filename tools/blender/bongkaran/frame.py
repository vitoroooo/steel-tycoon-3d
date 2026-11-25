"""The concrete skeleton: columns, beams and floor plates, whole on the west and broken off towards the east."""
import math
from kit.finish import ready
from kit.members import beam
from kit.parts import box
from kit.tubes import tube
from . import dims as d


def rebar(x, y, z, rng, count=4, reach=.9):
    """Reinforcing bars left sticking out where the concrete was broken away."""
    bars = []
    for k in range(count):
        dx, dy = (.12 if k % 2 else -.12), (.12 if k // 2 else -.12)
        tip = (x + dx + rng.uniform(-.35, .35), y + dy + rng.uniform(-.35, .35), z + rng.uniform(.4, reach))
        bars.append(tube('Rebar', [(x + dx, y + dy, z - .1), (x + dx, y + dy, z + .2), tip], .012, 'rebar', bend=.12, sides=5))
    return bars


def columns(rng):
    parts = []
    for i in range(d.BAYS_X + 1):
        for j in range(d.BAYS_Y + 1):
            floors = sum(d.standing(i, level) for level in range(1, d.FLOORS + 1))
            stub = rng.uniform(.5, 1.6) if floors < d.FLOORS else 0   # the jagged stump above the last whole storey
            height = floors * d.STOREY + stub
            x, y = d.column_x(i), d.column_y(j)
            parts.append(ready(box('Column', (d.COLUMN, d.COLUMN, height), (x, y, height / 2), 'concrete'), .015, 1.5, 'z'))
            if stub:
                parts += rebar(x, y, height, rng)
    return parts


def beams_and_slabs(rng):
    parts = []
    for level in range(1, d.FLOORS + 1):
        z = level * d.STOREY
        for i in range(d.BAYS_X + 1):
            if not d.standing(i, level):
                continue
            x = d.column_x(i)
            parts.append(ready(beam('Beam', (x, -d.DEPTH / 2, z - d.BEAM_DEPTH / 2), (x, d.DEPTH / 2, z - d.BEAM_DEPTH / 2),
                                    .3, d.BEAM_DEPTH, 'concrete'), .01, 1.5))
            if i == d.BAYS_X or not d.standing(i + 1, level):
                for j in range(d.BAYS_Y + 1):   # girders snapped off short of the missing bay
                    y, stub = d.column_y(j), rng.uniform(.5, 1.3)
                    parts.append(ready(box('Stub', (stub, .3, d.BEAM_DEPTH), (x + stub / 2, y, z - d.BEAM_DEPTH / 2), 'concrete'), .01, 1.5))
                    parts += rebar(x + stub, y, z - d.BEAM_DEPTH / 2, rng, 3, .5)
                continue
            for j in range(d.BAYS_Y + 1):
                y = d.column_y(j)
                parts.append(ready(beam('Girder', (x, y, z - d.BEAM_DEPTH / 2), (x + d.BAY_X, y, z - d.BEAM_DEPTH / 2),
                                        .3, d.BEAM_DEPTH, 'concrete'), .01, 1.5))
            for j in range(d.BAYS_Y):
                parts.append(ready(box('Slab', (d.BAY_X, d.BAY_Y, d.SLAB),
                                       (x + d.BAY_X / 2, d.column_y(j) + d.BAY_Y / 2, z + d.SLAB / 2), 'floor'), .008, 2))
    return parts


def hanging_slab():
    """One floor plate that came down at one end and still hangs from its reinforcement."""
    x, z, drop = d.column_x(2), 2 * d.STOREY, math.radians(38)
    centre = (x + math.cos(drop) * d.BAY_X / 2, d.column_y(0) + d.BAY_Y / 2, z - math.sin(drop) * d.BAY_X / 2)
    return [ready(box('HangingSlab', (d.BAY_X, d.BAY_Y * .92, d.SLAB), centre, 'floor', rot=(0, drop, 0)), .008, 2)]
