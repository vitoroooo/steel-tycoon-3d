"""The steel skeleton: portal frames, purlins, crane runway and wind bracing."""
from kit.finish import ready
from kit.members import beam
from kit.parts import box
from kit.tubes import tube
from . import dims as d


def portals():
    parts = []
    for x in d.FRAMES:
        for side in (-1, 1):
            y = side * (d.HALF_D - .22)
            parts.append(ready(box('Column', (.3, .44, d.EAVES), (x, y, d.EAVES / 2), 'frame'), .01, 2, 'z'))
            parts.append(ready(box('BasePlate', (.6, .7, .06), (x, y, .08), 'steel_dark'), .004))
            parts.append(ready(box('Haunch', (.3, 1.2, .5), (x, side * (d.HALF_D - .9), d.EAVES - .45), 'frame'), .01))
            rafter = beam('Rafter', (x, side * d.HALF_D, d.EAVES - .2), (x, 0, d.RIDGE - .2), .26, .42, 'frame')
            parts.append(ready(rafter, .01, 2, 'y'))
            parts.append(ready(box('Corbel', (.3, .7, .5), (x, side * (d.HALF_D - .75), d.CRANE_RAIL - .85), 'frame'), .01))
    return parts


def purlins():
    parts = []
    for side in (-1, 1):
        for step in range(1, 7):
            y = side * d.HALF_D * step / 6.5
            parts.append(ready(box('Purlin', (d.WIDTH, .08, .16), (0, y, d.roof_z(y) - .1), 'frame'), 0, 2))
        parts.append(ready(box('EavesBeam', (d.WIDTH, .2, .3), (0, side * (d.HALF_D - .2), d.EAVES - .2), 'frame'), .008, 2))
        for z in (3.2, 5.4):
            if side < 0:
                parts.append(ready(box('WallRail', (d.WIDTH, .08, .16), (0, -d.HALF_D + .06, z), 'frame'), 0, 2))
    return parts


def crane_runway():
    parts = []
    for side in (-1, 1):
        y = side * (d.HALF_D - .95)
        parts.append(ready(box('Runway', (d.WIDTH, .32, .6), (0, y, d.CRANE_RAIL - .3), 'crane'), .01, 2))
        parts.append(ready(box('RunwayRail', (d.WIDTH, .08, .1), (0, y, d.CRANE_RAIL + .05), 'steel_dark'), 0, 2))
    # The overhead crane itself, parked over the ladle bay.
    for x in (5.0, 6.6):
        parts.append(ready(box('Bridge', (.5, d.DEPTH - 1.6, .9), (x, 0, d.CRANE_RAIL + .6), 'crane'), .012, 2, 'y'))
    parts.append(ready(box('Crab', (2.3, 2.2, .7), (5.8, 1.5, d.CRANE_RAIL + 1.35), 'steel_dark'), .02))
    for y in (1.1, 1.9):
        parts.append(tube('HoistRope', [(5.8, y, d.CRANE_RAIL + 1), (5.8, y, 5.4)], .025, 'steel_dark', 0, 6))
    parts.append(ready(box('HookBlock', (.5, 1.1, .5), (5.8, 1.5, 5.2), 'crane'), .02))
    return parts


def bracing():
    """Crossed rods in the end bays of the closed wall and of the roof."""
    parts = []
    for near, far in ((d.FRAMES[0], d.FRAMES[1]), (d.FRAMES[4], d.FRAMES[5])):
        for a, b in ((near, far), (far, near)):
            parts.append(tube('WallBrace', [(a, -d.HALF_D + .2, d.DADO + .2), (b, -d.HALF_D + .2, d.EAVES - .6)],
                              .03, 'frame', 0, 6))
            for side in (-1, 1):
                parts.append(tube('RoofBrace', [(a, side * (d.HALF_D - .4), d.EAVES - .15), (b, side * .4, d.RIDGE - .3)],
                                  .025, 'frame', 0, 6))
    return parts
