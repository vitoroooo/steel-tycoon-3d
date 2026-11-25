"""What carries the load: chassis rails, tank, exhaust stack, mudguards and the wheels."""
from .. import shapes
from ..paint import cel
from ..rods import rod, wheel
from . import spec

R, T, TRACK = spec.WHEEL, spec.TYRE, spec.TRACK


def chassis(truck):
    nose, back, behind = spec.nose(truck), truck.length / 2, spec.cab_back(truck)
    parts = [shapes.block('Palang belakang', (0, back - .04, .8), (2.2, .12, .16), cel('navy'), .4),
             shapes.block('Pelat belakang', (0, back + .03, .8), (.5, .02, .16), cel('white'), .3),
             rod('Cerobong', (-.95, behind + .16, 1.0), (-.95, behind + .16, spec.ROOF + .2), .07, cel('steel'), 14, .4),
             rod('Tangki solar', (1.0, behind + .25, .78), (1.0, behind + 1.15, .78), .27, cel('steel'), 20, .35),
             shapes.block('Aki', (-1.0, behind + .6, .8), (.36, .6, .36), cel('navy'), .2)]
    for side in (-1, 1):
        parts += [
            shapes.block('Sasis', (side * .42, (nose + back) / 2 + .2, .9), (.14, truck.length - .5, .24), cel('navy'), .25),
            shapes.block('Lampu belakang', (side * .9, back + .03, .8), (.3, .03, .14), cel('red'), .35),
            shapes.block('Lumpur', (side * TRACK, truck.axles[-1] + R + .12, R + .06), (_width(truck, 1), .02, .5), cel('ink'), .2),
        ]
    return parts + guards(truck.axles, truck.dual)


def guards(axles, dual, first_steers=True):
    """A mudguard over every wheel position."""
    parts = []
    for i, y in enumerate(axles):
        wide = T * 2 + .14 if dual and (i > 0 or not first_steers) else T + .1
        for side in (-1, 1):
            parts.append(shapes.block('Spakbor', (side * TRACK, y, R * 2 + .07), (wide, R * 2 + .25, .07), cel('navy'), .45))
    return parts


def wheels(axles, dual, first_steers=True):
    """One list of parts per wheel position: axle by axle, left (+X) before right."""
    groups = []
    for i, y in enumerate(axles):
        twin = dual and (i > 0 or not first_steers)
        for side in (1, -1):
            offsets = (-(T / 2 + .02), T / 2 + .02) if twin else (0,)
            groups.append([part for dx in offsets for part in wheel((side * TRACK + dx, y, R), R, T, 'yellow')])
    return groups


def _width(truck, axle):
    return T * 2 + .1 if truck.dual and axle > 0 else T + .06
