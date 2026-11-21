"""The digging arm, fitted with a hydraulic breaker: boom, stick, hammer and the rams that drive them."""
from mathutils import Vector
from kit.finish import ready
from kit.members import beam
from kit.parts import cylinder
from kit.tubes import tube
from . import dims as d


def ram(start, end, bore=.075):
    """A hydraulic cylinder: the dark barrel for the first 55 % of its length, then the bright rod."""
    a, b = Vector(start), Vector(end)
    mid = a.lerp(b, .55)
    return [tube('RamBarrel', [tuple(a), tuple(mid)], bore, 'steel_dark', bend=0, sides=10),
            tube('RamRod', [tuple(mid), tuple(b)], bore * .5, 'chrome', bend=0, sides=8)]


def pin(at):
    return ready(cylinder('Pin', .11, .5, at, 'steel_dark', (0, 1.5708, 0), 14), .006)


def arm():
    x = d.BOOM_FOOT[0]
    parts = [ready(beam('BoomLower', d.BOOM_FOOT, d.ELBOW, .34, .5, 'paint_yellow'), .03),
             ready(beam('BoomUpper', d.ELBOW, d.BOOM_TIP, .32, .44, 'paint_yellow'), .03),
             ready(beam('Stick', d.BOOM_TIP, d.STICK_END, .28, .36, 'paint_yellow'), .03)]
    parts += [pin(d.BOOM_FOOT), pin(d.ELBOW), pin(d.BOOM_TIP), pin(d.STICK_END)]
    for side in (-.27, .27):   # the pair of boom rams either side of the foot
        parts += ram((x + side, 1.75, d.DECK + .3), (x + side, 2.75, 3.25))
    parts += ram((x, 3.75, 4.72), (x, 5.35, 4.22), .07)             # stick ram, on the boom's back
    parts += ram((x, 5.75, 3.45), (x, 6.42, 2.05), .06)             # tool ram, on the stick's face
    housing_top = Vector(d.STICK_END) + Vector((0, .05, .12))
    housing_end = Vector(d.CHISEL_TIP) + Vector((0, -.14, .68))
    parts.append(ready(beam('BreakerHousing', tuple(housing_top), tuple(housing_end), .3, .3, 'steel_dark'), .025))
    parts.append(tube('Chisel', [tuple(housing_end), d.CHISEL_TIP], .05, 'chrome', bend=0, sides=10))
    hose = [(x + .1, 1.3, d.DECK + .9), (x + .19, 2.6, 3.6), (x + .19, 3.6, 4.6), (x + .18, 5.4, 4.0), (x + .17, 6.1, 2.4)]
    parts.append(tube('Hose', hose, .022, 'hose', bend=.3, sides=6))
    return parts
