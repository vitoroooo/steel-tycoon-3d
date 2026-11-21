"""Everything that turns with the handlebar: fork, mudguard, headlamp and controls."""
import math
from kit.finish import ready
from kit.parts import box, cylinder, ring
from kit.shells import arc_band
from kit.tubes import tube
from . import dims as d

FACING = (math.pi / 2, 0, 0)   # turns a Z-axis part so it points along Y
SIDEWAYS = (0, math.pi / 2, 0)
CLAMP = (d.HEAD_Y + .03, d.HEAD_Z - .2)   # lower triple clamp (y, z)


def fork():
    parts = []
    axle = (d.FRONT_AXLE_Y, d.WHEEL_RADIUS)
    knee = (CLAMP[0] + (axle[0] - CLAMP[0]) * .5, CLAMP[1] + (axle[1] - CLAMP[1]) * .5)
    for x in (-d.FORK_X, d.FORK_X):
        top = (x, d.HEAD_Y, d.HEAD_Z + .02)
        parts.append(ready(tube('Stanchion', [top, (x, *knee)], .015, 'chrome', bend=0), 0, .3))
        parts.append(ready(tube('Slider', [(x, *knee), (x, *axle)], .023, 'engine', bend=0, sides=12), 0, .3))
        parts.append(ready(tube('Gaiter', [(x, knee[0] - .05, knee[1] + .1), (x, *knee)], .027, 'rubber', bend=0), 0, .2))
    for y, z in ((d.HEAD_Y, d.HEAD_Z), CLAMP):
        parts.append(ready(box('Clamp', (d.FORK_X * 2 + .07, .075, .03), (0, y, z), 'engine', (-.47, 0, 0)), .006, .3))
    parts.append(ready(tube('Stem', [(0, d.HEAD_Y - .01, d.HEAD_Z + .03), (0, CLAMP[0], CLAMP[1])], .028, 'frame', bend=0), 0, .3))
    parts.append(ready(cylinder('Axle', .011, d.FORK_X * 2 + .08, (0, *axle), 'steel', SIDEWAYS, 10), .002, .3))
    guard = arc_band('Mudguard', d.WHEEL_RADIUS + .035, .15, 20, 175, (0, *axle), 'paint', crown=.03)
    parts.append(ready(guard, 0, .5))
    for x in (-d.FORK_X, d.FORK_X):   # mudguard stays
        stay = [(x, d.FRONT_AXLE_Y, d.WHEEL_RADIUS), (x * .75, d.FRONT_AXLE_Y - .26, d.WHEEL_RADIUS + .14)]
        parts.append(ready(tube('Stay', stay, .005, 'steel', bend=0, sides=6), 0, .3))
    return parts


def headlamp():
    y, z = d.HEAD_Y + .15, d.HEAD_Z - .04
    parts = [ready(cylinder('LampShell', .095, .12, (0, y, z), 'paint', FACING, 28), .02, .4)]
    parts.append(ready(ring('LampBezel', .09, .011, (0, y + .06, z), 'chrome', FACING, 1, 32), 0, .3))
    parts.append(ready(cylinder('LampLens', .086, .012, (0, y + .062, z), 'glass', FACING, 28), .004, .2))
    parts.append(ready(box('Cowl', (.22, .05, .09), (0, y - .03, z + .12), 'paint', (.5, 0, 0)), .012, .4))
    parts.append(ready(cylinder('Speedo', .05, .04, (0, d.HEAD_Y + .02, d.HEAD_Z + .1), 'frame', (.6, 0, 0), 20), .004, .3))
    parts.append(ready(cylinder('SpeedoFace', .043, .004, (0, d.HEAD_Y + .008, d.HEAD_Z + .117), 'plate', (.6, 0, 0), 20), 0, .2))
    for x in (-.19, .19):   # indicators on rubber stalks
        parts.append(ready(tube('Stalk', [(x * .5, y - .02, z), (x, y, z)], .007, 'rubber', bend=0, sides=6), 0, .2))
        parts.append(ready(cylinder('Indicator', .028, .05, (x, y + .01, z), 'lamp_amber', FACING, 14), .006, .2))
    return parts


def handlebar():
    z, y = d.BAR_Z, d.HEAD_Y - .1
    half = [(.1, d.HEAD_Y, d.HEAD_Z + .05), (.17, d.HEAD_Y - .03, z), (.4, y, z)]
    path = [(-px, py, pz) for px, py, pz in half[::-1]] + half
    parts = [ready(tube('Bar', path, .011, 'chrome', bend=.05), 0, .3)]
    for side in (-1, 1):
        parts.append(ready(cylinder('Grip', .017, .12, (side * .34, y, z), 'rubber', SIDEWAYS, 14), .003, .2))
        parts.append(ready(box('Switch', (.05, .035, .04), (side * .245, y, z), 'frame'), .006, .2))
        lever = [(side * .25, y + .03, z), (side * .3, y + .055, z), (side * .4, y + .05, z - .005)]
        parts.append(ready(tube('Lever', lever, .005, 'engine', bend=.02, sides=6), 0, .2))
        stalk = [(side * .23, y, z + .02), (side * .25, y - .02, z + .17), (side * .31, y - .03, z + .2)]
        parts.append(ready(tube('MirrorStalk', stalk, .005, 'frame', bend=.03, sides=6), 0, .2))
        parts.append(ready(cylinder('Mirror', .055, .014, (side * .34, y - .03, z + .21), 'frame', FACING, 20), .005, .2))
        parts.append(ready(cylinder('MirrorGlass', .048, .003, (side * .34, y - .038, z + .21), 'chrome', FACING, 20), 0, .2))
    return parts
