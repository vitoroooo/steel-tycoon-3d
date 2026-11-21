"""The tail: a fan of stiff main feathers and, over them, the long sickles that arch and droop."""
from .dims import TAIL_ROOT
from .feathers import plume


def tail():
    x, y, z = TAIL_ROOT
    parts = []
    for i in range(7):          # main tail feathers, fanned in the vertical plane
        lift = i / 6
        tip = (0, y - .13 - lift * .05, z + .05 + lift * .2)
        for side in (-1, 1):
            root = (side * .012, y + .03, z - .02 + lift * .03)
            bend = (side * (.02 + lift * .01), y - .06, z + .03 + lift * .12)
            parts.append(plume('Main', root, bend, (side * .012, tip[1], tip[2]), .03, (1, 0, 0), 'feather_tail'))
    sickles = ((.0, .33, .27, -.33), (.022, .29, .25, -.3), (-.022, .28, .24, -.27),
               (.04, .22, .2, -.21), (-.04, .21, .19, -.19))            # x, arch height, reach, droop
    for sx, arch, reach, droop in sickles:
        root = (sx * .4, y + .02, z + .01)
        bend = (sx, y - reach * .55, z + arch)
        tip = (sx * 1.6, y - reach - .1, z + arch + droop)
        parts.append(plume('Sickle', root, bend, tip, .026, (1, 0, .15), 'feather_tail'))
    return parts
