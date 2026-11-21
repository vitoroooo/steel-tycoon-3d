"""What makes a ruko look lived in: air conditioner, water tank, antenna, meter and pipes."""
from kit.finish import ready
from kit.parts import box, cylinder
from kit.tubes import tube
from . import dims as d

SIDEWAYS = (0, 1.5708, 0)
FACING = (1.5708, 0, 0)


def air_conditioner(x, y, z, facing=1):
    """An outdoor unit on angle brackets with its refrigerant line running into the wall."""
    parts = [ready(box('ACUnit', (.8, .3, .55), (x, y + facing * .22, z + .3), 'appliance'), .012, 1),
             ready(cylinder('ACFan', .21, .02, (x - .1, y + facing * .375, z + .3), 'frame', FACING, 16), 0, 1)]
    for dx in (-.3, .3):
        parts.append(ready(box('ACBracket', (.03, .4, .03), (x + dx, y + facing * .2, z + .015), 'steel'), 0, 1))
    line = [(x + .42, y + facing * .2, z + .35), (x + .6, y + facing * .2, z + .35), (x + .6, y + facing * .03, z + .9)]
    parts.append(ready(tube('ACLine', line, .012, 'pipe', bend=.04, sides=6), 0, 1))
    return parts


def water_tank(floors):
    """The orange toren on a steel stand; every roof in Sukabaja has one."""
    top = d.roof_z(floors) + .14
    x, y, legs = 1.1, -2.2, 1.5
    parts = [ready(cylinder('Tank', .55, 1.1, (x, y, top + legs + .6), 'toren', segments=16), .02, 1),
             ready(cylinder('TankCap', .2, .1, (x, y, top + legs + 1.2), 'toren', segments=12), 0, 1),
             ready(box('TankDeck', (1.3, 1.3, .05), (x, y, top + legs), 'steel'), 0, 1)]
    for sx in (-.6, .6):
        for sy in (-.6, .6):
            parts.append(ready(box('TankLeg', (.05, .05, legs), (x + sx, y + sy, top + legs / 2), 'steel'), 0, 1))
        brace = [(x + sx, y - .6, top + .1), (x + sx, y + .6, top + legs - .1)]
        parts.append(ready(tube('TankBrace', brace, .012, 'steel', bend=0, sides=5), 0, 1))
    feed = [(x - .5, y, top + legs + .2), (x - .9, y, top + legs + .2), (x - .9, y, top + .1), (x - .9, d.BACK + .1, top + .1)]
    parts.append(ready(tube('TankFeed', feed, .02, 'pipe', bend=.06, sides=6), 0, 1))
    return parts


def antenna(floors):
    top = d.roof_z(floors) + .14
    x, y = -1.6, 1.5
    parts = [ready(tube('Mast', [(x, y, top), (x, y, top + 3.2)], .018, 'steel', bend=0, sides=6), 0, 1),
             ready(tube('Boom', [(x - .6, y, top + 3.0), (x + .6, y, top + 3.0)], .01, 'steel', bend=0, sides=5), 0, 1)]
    for i in range(6):
        px, half = x - .55 + i * .22, .32 - i * .03
        parts.append(ready(tube('Element', [(px, y - half, top + 3.0), (px, y + half, top + 3.0)], .005, 'steel',
                                bend=0, sides=4), 0, 1))
    return parts


def downpipes(floors):
    top = d.roof_z(floors)
    parts = []
    for x in (-d.HALF + .22, d.HALF - .22):
        run = [(x, d.BACK - .1, top + .1), (x, d.BACK - .1, .5), (x, d.BACK - .35, .12)]
        parts.append(ready(tube('Downpipe', run, .045, 'pipe', bend=.08, sides=8), 0, 1))
        for z in (1.2, top - .6):
            parts.append(ready(box('PipeClamp', (.14, .03, .04), (x, d.BACK - .06, z), 'steel'), 0, 1))
    return parts
