"""A spoked bicycle wheel, built around the origin with its axle along X."""
import math
from kit.finish import ready
from kit.parts import cylinder, ring
from kit.tubes import tube
from . import dims as d

SIDEWAYS = (0, math.pi / 2, 0)   # turns a Z-axis part so it lies along X
SPOKES = 28


def _spokes():
    parts = []
    for index in range(SPOKES):
        flange = .03 if index % 2 else -.03
        lean = .55 if (index // 2) % 2 else -.55      # crossed lacing
        hub_angle = index / SPOKES * math.tau
        rim_angle = hub_angle + lean
        start = (flange, math.cos(hub_angle) * .034, math.sin(hub_angle) * .034)
        end = (0, math.cos(rim_angle) * .268, math.sin(rim_angle) * .268)
        parts.append(tube('Spoke', [start, end], .0022, 'spoke', bend=0, sides=4))
    return parts


def wheel():
    """Returns the parts of one wheel centred on the origin."""
    tyre = ring('Tyre', d.WHEEL_RADIUS - .03, .03, (0, 0, 0), 'rubber', SIDEWAYS, 1.0, 56)
    rim = ring('Rim', d.WHEEL_RADIUS - .058, .016, (0, 0, 0), 'steel', SIDEWAYS, 1.25)
    hub = cylinder('Hub', .022, .1, (0, 0, 0), 'steel', SIDEWAYS, 16)
    parts = [ready(tyre, 0, .25), ready(rim, 0, .4), ready(hub, .002, .4)]
    for x in (-.03, .03):
        parts.append(ready(cylinder('Flange', .04, .006, (x, 0, 0), 'steel', SIDEWAYS, 20), .001, .4))
    for index in range(40):   # raised tread blocks around the tyre
        angle = index / 40 * math.tau
        radius = d.WHEEL_RADIUS - .004
        block = cylinder('Tread', .012, .034, (0, math.cos(angle) * radius, math.sin(angle) * radius),
                         'rubber', SIDEWAYS, 6)
        parts.append(ready(block, 0, .25))
    return parts + [ready(s, 0, .4) for s in _spokes()]
