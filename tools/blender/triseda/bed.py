"""The cargo half: ladder chassis, rear axle and a steel tipper-style bed."""
import math
from kit.finish import ready
from kit.parts import blob, box, cylinder
from kit.tubes import tube
from . import dims as d

SIDEWAYS = (0, math.pi / 2, 0)
LENGTH = d.BED_FRONT - d.BED_BACK
MIDDLE = (d.BED_FRONT + d.BED_BACK) / 2
TOP = d.BED_FLOOR + d.BED_WALL
SHEET = .012


def chassis():
    parts = []
    for x in (-d.RAIL_X, d.RAIL_X):
        parts.append(ready(box('Rail', (.05, LENGTH + .35, .07), (x, MIDDLE + .17, d.BED_FLOOR - .06), 'frame'), .004, .5))
        for index, length in enumerate((.7, .54, .38)):   # leaf spring pack
            z = d.WHEEL_RADIUS + .075 - index * .011
            parts.append(ready(box('Leaf', (.05, length, .008), (x, d.REAR_AXLE_Y, z), 'steel'), .002, .3))
        for y in (-.33, .33):
            shackle = box('Shackle', (.012, .03, .2), (x, d.REAR_AXLE_Y + y, d.WHEEL_RADIUS + .16), 'steel')
            parts.append(ready(shackle, .003, .3))
    for y in (d.BED_BACK + .1, d.REAR_AXLE_Y, MIDDLE + .25, d.BED_FRONT):
        parts.append(ready(box('CrossMember', (d.RAIL_X * 2, .04, .05), (0, y, d.BED_FLOOR - .05), 'frame'), .004, .5))
    parts.append(ready(cylinder('AxleTube', .032, d.REAR_X * 2 - .1, (0, d.REAR_AXLE_Y, d.WHEEL_RADIUS), 'frame', SIDEWAYS, 14), .003, .3))
    parts.append(ready(blob('Differential', (.2, .22, .2), (0, d.REAR_AXLE_Y, d.WHEEL_RADIUS), 'engine', bulge=.8), 0, .3))
    shaft = [(0, d.REAR_AXLE_Y + .1, d.WHEEL_RADIUS), (0, .52, .33)]
    parts.append(ready(tube('PropShaft', shaft, .019, 'steel', bend=0), 0, .3))
    return parts


def _panel(name, size, at, ribs, along):
    """A pressed-steel wall: sheet, rolled top edge and evenly spaced stiffening ribs."""
    parts = [ready(box(name, size, at, 'bed'), .003, .9, 'z')]
    span = size[0] if along == 'x' else size[1]
    out = 1 if (at[0] if along == 'y' else at[1]) >= 0 else -1
    for index in range(ribs):
        offset = (index + .5) / ribs * span - span / 2
        if along == 'x':
            rib_at, rib_size = (at[0] + offset, at[1] + out * .014, at[2] - .01), (.035, .022, size[2] - .06)
        else:
            rib_at, rib_size = (at[0] + out * .014, at[1] + offset, at[2] - .01), (.022, .035, size[2] - .06)
        parts.append(ready(box('Rib', rib_size, rib_at, 'bed'), .006, .9, 'z'))
    return parts


def bed():
    w, mid_z = d.BED_HALF_WIDTH, d.BED_FLOOR + d.BED_WALL / 2
    parts = [ready(box('Floor', (w * 2, LENGTH, .02), (0, MIDDLE, d.BED_FLOOR), 'bedfloor'), .003, .6)]
    for x in (-w, w):
        parts += _panel('SideWall', (SHEET, LENGTH, d.BED_WALL), (x, MIDDLE, mid_z), 7, 'y')
        rail = [(x, d.BED_BACK, TOP), (x, d.BED_FRONT, TOP)]
        parts.append(ready(tube('TopRail', rail, .016, 'bed', bend=0), 0, .6))
        for y in (d.BED_BACK, d.BED_FRONT):
            parts.append(ready(box('Post', (.04, .04, d.BED_WALL + .02), (x, y, mid_z), 'bed'), .006, .6))
    parts += _panel('Tailgate', (w * 2 - .05, SHEET, d.BED_WALL - .02), (0, d.BED_BACK, mid_z), 5, 'x')
    parts += _panel('FrontWall', (w * 2, SHEET, d.BED_WALL), (0, d.BED_FRONT, mid_z), 5, 'x')
    for x in (-w + .1, 0, w - .1):
        hinge = cylinder('Hinge', .014, .12, (x, d.BED_BACK - .012, d.BED_FLOOR + .02), 'steel', SIDEWAYS, 10)
        parts.append(ready(hinge, .002, .3))
    for x in (-w, w):   # drop latches holding the tailgate
        parts.append(ready(box('Latch', (.03, .09, .03), (x, d.BED_BACK + .03, TOP - .08), 'steel'), .004, .3))
        parts.append(ready(cylinder('LatchPin', .008, .08, (x, d.BED_BACK + .03, TOP - .03), 'steel', segments=8), .001, .3))
    return parts


def headboard():
    """A pipe guard between rider and load, with wire mesh so cargo cannot slide forward."""
    x, y, top = d.BED_HALF_WIDTH - .03, d.BED_FRONT + .02, 1.52
    hoop = [(-x, y, TOP), (-x, y, top), (x, y, top), (x, y, TOP)]
    parts = [ready(tube('GuardHoop', hoop, .018, 'frame', bend=.09), 0, .5)]
    for z in (TOP + .22, TOP + .45):
        parts.append(ready(tube('GuardBar', [(-x, y, z), (x, y, z)], .012, 'frame', bend=0, sides=8), 0, .5))
    for index in range(1, 12):
        wire_x = -x + index * x / 6
        parts.append(ready(tube('Wire', [(wire_x, y, TOP), (wire_x, y, top)], .004, 'steel', bend=0, sides=4), 0, .3))
    return parts
