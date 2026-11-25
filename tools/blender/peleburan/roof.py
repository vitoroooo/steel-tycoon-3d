"""Roof sheets, daylight strips, the ridge monitor that lets furnace fumes out, gutters and downpipes."""
import math
from kit.finish import ready
from kit.members import beam
from kit.parts import box
from kit.tubes import tube
from . import dims as d

OVERHANG = .5


def slopes():
    parts = []
    for side in (-1, 1):
        eaves = (0, side * (d.HALF_D + OVERHANG), d.roof_z(d.HALF_D + OVERHANG) + .05)
        ridge = (0, side * d.MONITOR_HALF * .6, d.roof_z(d.MONITOR_HALF * .6) + .05)
        parts.append(ready(beam('RoofSheet', eaves, ridge, d.WIDTH + .8, .05, 'roof'), 0, 2, 'y'))
        for x in (-10.5, -4.5, 1.5, 7.5):
            low = (x, side * (d.HALF_D - .6), d.roof_z(d.HALF_D - .6) + .09)
            high = (x, side * 3, d.roof_z(3) + .09)
            parts.append(ready(beam('RoofLight', low, high, 1.1, .03, 'skylight'), 0, 2, 'y'))
    return parts


def monitor():
    parts = []
    base = d.roof_z(d.MONITOR_HALF)
    top = base + d.MONITOR_RISE
    length = d.WIDTH - 3
    for side in (-1, 1):
        y = side * d.MONITOR_HALF
        parts.append(ready(box('VentLouvre', (length, .08, d.MONITOR_RISE), (0, y, base + d.MONITOR_RISE / 2), 'louvre'),
                           0, .25, 'x'))
        sheet = beam('VentRoof', (0, side * (d.MONITOR_HALF + .35), top - .05), (0, 0, top + .5), length + .5, .05, 'roof')
        parts.append(ready(sheet, 0, 2, 'y'))
    for x in (-length / 2, length / 2):
        parts.append(ready(box('VentEnd', (.06, d.MONITOR_HALF * 2, d.MONITOR_RISE + .3),
                               (x, 0, base + d.MONITOR_RISE / 2 + .1), 'seng'), 0, 2, 'z'))
    return parts


def drainage():
    parts = []
    for side in (-1, 1):
        y = side * (d.HALF_D + OVERHANG + .08)
        z = d.roof_z(d.HALF_D + OVERHANG) - .05
        parts.append(ready(box('Gutter', (d.WIDTH + .8, .2, .16), (0, y, z), 'steel_dark'), .01, 2))
        for x in (-d.HALF_W + .5, d.HALF_W - .5):
            wall = side * (d.HALF_D + .12)
            parts.append(tube('Downpipe', [(x, y, z), (x, wall, z - .7), (x, wall, .3), (x, wall + side * .25, .12)],
                              .06, 'steel_dark', .12, 8))
    return parts


def lamps():
    """High-bay lights hung from the rafters; Unity makes the lens glow."""
    parts = []
    for x in (-9, -3, 3, 9):
        for y in (-4.5, 4.5):
            z = d.roof_z(y) - 1.6
            parts.append(tube('LampDrop', [(x, y, d.roof_z(y) - .3), (x, y, z + .2)], .012, 'steel_dark', 0, 6))
            parts.append(ready(box('LampShade', (.5, .5, .22), (x, y, z + .1), 'steel_dark'), .03))
            parts.append(ready(box('LampLens', (.4, .4, .03), (x, y, z - .02), 'lamp'), 0))
    return parts
