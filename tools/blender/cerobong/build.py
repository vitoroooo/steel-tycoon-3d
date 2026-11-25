"""A 30 m steel stack on a concrete plinth: flanged sections, warning bands, caged ladder and two platforms."""
import math
from kit import scene
from kit.finish import ready
from kit.members import frustum
from kit.parts import box, cylinder, ring
from kit.tubes import tube

HEIGHT, BOTTOM, TOP, PLINTH = 30.0, 1.5, .95, 1.4
SECTIONS = ((0, 6, 'stack'), (6, 12, 'stack'), (12, 18, 'stack'), (18, 21, 'stack'),
            (21, 24, 'band_red'), (24, 27, 'band_white'), (27, 30, 'band_red'))
PLATFORMS = (12.0, 24.0)


def radius(z):
    return BOTTOM + (TOP - BOTTOM) * z / HEIGHT


def shaft():
    parts = [ready(cylinder('Plinth', 2.3, PLINTH, (0, 0, PLINTH / 2), 'concrete', segments=8), .04, 2)]
    for low, high, slot in SECTIONS:
        section = frustum('Section', radius(low), radius(high), high - low, (0, 0, PLINTH + (low + high) / 2), slot, 28)
        parts.append(ready(section, 0, 2, 'z'))
        parts.append(ring('Flange', radius(high) + .03, .05, (0, 0, PLINTH + high), 'steel_dark', flatten=.6, segments=28))
    parts.append(cylinder('Beacon', .09, .2, (0, TOP + .12, PLINTH + HEIGHT - .3), 'beacon', segments=10))
    for index in range(8):      # anchor bolts
        angle = index * math.pi / 4
        parts.append(cylinder('Anchor', .05, .16, (math.cos(angle) * 1.75, math.sin(angle) * 1.75, PLINTH + .08),
                              'steel_dark', segments=8))
    duct = [(0, -1.1, PLINTH + 3), (0, -2.6, PLINTH + 3), (0, -3.4, 9.6), (0, -6, 9.6)]   # up to the hall's gable
    parts.append(tube('Inlet', duct, .6, 'duct', .7, 16))
    for y in (-2.2,):
        parts.append(ready(box('DuctStand', (.14, .14, PLINTH + 2.4), (0, y, (PLINTH + 2.4) / 2), 'steel_dark'), .008))
    return parts


def ladder():
    """Runs up the +Y face; the cage starts above head height."""
    parts = []
    top = PLINTH + HEIGHT - 1
    for x in (-.22, .22):
        parts.append(tube('Stile', [(x, radius(0) + .2, .3), (x, radius(HEIGHT) + .2, top)], .02, 'steel_dark', 0, 6))
    rungs = int((top - .5) / .32)
    for index in range(rungs):
        z = .5 + index * .32
        y = radius(z - PLINTH) + .2
        parts.append(tube('Rung', [(-.22, y, z), (.22, y, z)], .012, 'steel_dark', 0, 6))
        if index % 4 == 0 and z > 3:
            parts.append(ring('Hoop', .36, .014, (0, y + .3, z), 'steel_dark', segments=16))
    return parts


def platforms():
    parts = []
    for level in PLATFORMS:
        z = PLINTH + level
        reach = radius(level) + .55
        parts.append(ring('Walkway', reach, .5, (0, 0, z), 'deck', flatten=.06, segments=32))
        for height in (.55, 1.05):
            parts.append(ring('Guard', reach + .45, .02, (0, 0, z + height), 'rail', segments=32))
        for index in range(10):
            angle = index * math.pi / 5
            x, y = math.cos(angle) * (reach + .45), math.sin(angle) * (reach + .45)
            parts.append(tube('GuardPost', [(x, y, z), (x, y, z + 1.05)], .02, 'rail', 0, 6))
            parts.append(tube('Bracket', [(x * .55, y * .55, z - .7), (x, y, z - .04)], .025, 'steel_dark', 0, 6))
    return parts


def build():
    root = scene.root('Cerobong')
    scene.join('Body', shaft() + ladder() + platforms(), root)
    return 'Props', 'Cerobong'
