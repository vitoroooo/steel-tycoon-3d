"""The lane face: tiled porch, panel door, shuttered window, a bamboo bench and a hanging bird cage."""
from kit.finish import ready
from kit.parts import box, cylinder
from kit.tubes import tube
from . import dims as d

Y = d.FRONT          # plane of the front wall


def porch_floor():
    depth = d.PORCH
    return [ready(box('PorchFloor', (d.WIDTH, depth, .15), (0, Y + depth / 2, .075), 'tile'), 0, 1.2),
            ready(box('PorchStep', (1.6, .3, .08), (-1.2, Y + depth + .15, .04), 'concrete'), 0, 2)]


def door():
    x, z = -1.2, d.PLINTH
    parts = [ready(box('DoorFrame', (1.04, .1, 2.16), (x, Y + .01, z + 1.08), 'frame'), 0, 1),
             ready(box('Door', (.88, .05, 2.04), (x, Y + .05, z + 1.02), 'wood'), .006, 1, grain='z')]
    for dz in (.55, 1.5):   # raised panels
        parts.append(ready(box('DoorPanel', (.62, .02, .7), (x, Y + .085, z + dz), 'wood'), .01, 1, grain='z'))
    parts.append(ready(cylinder('DoorKnob', .03, .05, (x + .33, Y + .1, z + 1.0), 'steel', (1.5708, 0, 0), 8), 0, 1))
    parts.append(ready(box('Transom', (.88, .04, .3), (x, Y + .04, z + 2.3), 'glass'), 0, 1))
    return parts


def window():
    x, z = 1.3, d.PLINTH + 1.5
    parts = [ready(box('WindowFrame', (1.5, .1, 1.3), (x, Y + .01, z), 'frame'), 0, 1),
             ready(box('WindowGlass', (1.36, .04, 1.16), (x, Y + .04, z), 'glass'), 0, 1),
             ready(box('Mullion', (.04, .06, 1.16), (x, Y + .05, z), 'frame'), 0, 1),
             ready(box('WindowSill', (1.64, .16, .05), (x, Y + .07, z - .68), 'trim'), 0, 1)]
    for side in (-1, 1):   # shutters folded back against the wall
        sx = x + side * 1.12
        parts.append(ready(box('Shutter', (.68, .035, 1.2), (sx, Y + .03, z), 'shutter'), .004, 1, grain='z'))
        for i in range(8):
            parts.append(ready(box('ShutterSlat', (.56, .02, .035), (sx, Y + .055, z - .49 + i * .14), 'shutter'), 0, 1))
    return parts


def bench():
    """A bale-bale: the bamboo bench every porch has for sitting out the afternoon."""
    x, y, z = 1.3, Y + .55, .15
    parts = [ready(box('BenchSeat', (1.6, .55, .04), (x, y, z + .42), 'bamboo'), .004, .5)]
    for dx in (-.72, .72):
        for dy in (-.22, .22):
            parts.append(ready(cylinder('BenchLeg', .03, .42, (x + dx, y + dy, z + .21), 'bamboo', segments=8), 0, .5))
    for i in range(7):
        rail = [(x - .78, y - .24 + i * .08, z + .45), (x + .78, y - .24 + i * .08, z + .45)]
        parts.append(ready(tube('BenchSlat', rail, .018, 'bamboo', bend=0, sides=6), 0, .5))
    return parts


def bird_cage():
    x, y, z = -2.3, Y + d.PORCH - .5, 1.75
    parts = [ready(cylinder('CageBase', .17, .04, (x, y, z), 'bamboo', segments=12), 0, .5),
             ready(cylinder('CageTop', .17, .03, (x, y, z + .42), 'bamboo', segments=12), 0, .5),
             ready(tube('CageHook', [(x, y, z + .43), (x, y, z + .72)], .006, 'steel', bend=0, sides=4), 0, 1)]
    for i in range(12):
        from math import cos, sin, tau
        bx, by = x + .16 * cos(tau * i / 12), y + .16 * sin(tau * i / 12)
        parts.append(ready(tube('CageBar', [(bx, by, z), (bx, by, z + .42)], .004, 'bamboo', bend=0, sides=4), 0, .5))
    return parts
