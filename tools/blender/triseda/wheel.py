"""A small fat motorcycle wheel with a pressed-steel rim, axle along X, centred on the origin."""
import math
from kit.finish import ready
from kit.parts import box, cylinder, ring
from . import dims as d

SIDEWAYS = (0, math.pi / 2, 0)
BLOCKS = 30


def _tread():
    """Two staggered rows of knobs, the pattern of a cheap cargo tyre."""
    parts = []
    for index in range(BLOCKS):
        for row, x in enumerate((-.022, .022)):
            angle = (index + row * .5) / BLOCKS * math.tau
            reach = d.WHEEL_RADIUS - .006
            knob = box('Knob', (.03, .034, .014), (x, math.cos(angle) * reach, math.sin(angle) * reach),
                       'rubber', (angle - math.pi / 2, 0, 0))
            parts.append(ready(knob, 0, .25))
    return parts


def wheel(lugs=4):
    inner = d.WHEEL_RADIUS - d.TYRE
    tyre = ring('Tyre', inner, d.TYRE, (0, 0, 0), 'rubber', SIDEWAYS, 1.05, 48)
    rim = ring('Rim', inner - .05, .017, (0, 0, 0), 'rim', SIDEWAYS, 2.6, 40)
    parts = [ready(tyre, 0, .25), ready(rim, 0, .3)]
    parts.append(ready(cylinder('Disc', inner - .05, .012, (0, 0, 0), 'rim', SIDEWAYS, 32), .002, .3))
    parts.append(ready(cylinder('Drum', .075, .07, (0, 0, 0), 'engine', SIDEWAYS, 24), .004, .3))
    parts.append(ready(cylinder('Cap', .028, .1, (0, 0, 0), 'steel', SIDEWAYS, 12), .003, .3))
    for index in range(lugs):
        angle = index / lugs * math.tau + .4
        y, z = math.cos(angle) * .052, math.sin(angle) * .052
        for x in (-.04, .04):
            parts.append(ready(cylinder('Lug', .009, .012, (x, y, z), 'steel', SIDEWAYS, 6), .001, .3))
    for index in range(5):   # pressed slots in the disc
        angle = index / 5 * math.tau
        y, z = math.cos(angle) * .105, math.sin(angle) * .105
        slot = cylinder('Slot', .016, .016, (0, y, z), 'rubber', SIDEWAYS, 10)
        parts.append(ready(slot, 0, .3))
    return parts + _tread()
