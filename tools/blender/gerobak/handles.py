"""Push bar, resting legs and the axle that hangs under the box."""
import math
from kit.finish import ready
from kit.parts import box, cylinder
from kit.tubes import tube
from . import dims as d

SIDEWAYS = (0, math.pi / 2, 0)


def push_bar():
    """One pipe bent into a U: along both sides of the box and across at the grip."""
    x, low = d.HALF_WIDTH - .05, d.FLOOR - d.BAR * 1.5
    side = [(x, .25, low), (x, -d.HALF_LENGTH - .08, low), (x, GRIP_RISE_Y, d.GRIP_Z), (x, d.GRIP_Y, d.GRIP_Z)]
    path = [(-px, py, pz) for px, py, pz in side] + side[::-1]
    parts = [ready(tube('PushBar', path, .017, 'frame', bend=.09), 0, .6)]
    for grip_x in (-.2, .2):   # strips of inner tube wound on as grips
        parts.append(ready(cylinder('Grip', .0215, .15, (grip_x, d.GRIP_Y, d.GRIP_Z), 'rubber', SIDEWAYS, 14), .002, .2))
    return parts


GRIP_RISE_Y = d.GRIP_Y + .16


def legs():
    parts = []
    for side in (-1, 1):
        x = side * (d.HALF_WIDTH - .05)
        path = [(x, -d.HALF_LENGTH - .22, d.FLOOR + .1), (x, -d.HALF_LENGTH - .3, .012)]
        parts.append(ready(tube('Leg', path, .014, 'frame', bend=0), 0, .6))
        parts.append(ready(cylinder('Foot', .032, .012, (x, -d.HALF_LENGTH - .3, .006), 'steel', segments=14), .002, .4))
        brace = [(x, -d.HALF_LENGTH - .02, d.FLOOR - d.BAR), (x, -d.HALF_LENGTH - .27, .2)]
        parts.append(ready(tube('Brace', brace, .008, 'frame', bend=0, sides=6), 0, .6))
    return parts


def axle():
    """Axle with two flat brackets welded to the ribs under the floor."""
    parts = [ready(cylinder('Axle', .013, d.WHEEL_X * 2 + .09, (0, 0, d.WHEEL_RADIUS), 'steel', SIDEWAYS, 12), .002, .4)]
    drop = d.FLOOR - d.BAR * 2 - d.WHEEL_RADIUS
    for x in (-.2, .2):
        parts.append(ready(box('Bracket', (.008, .06, drop + .03), (x, 0, d.WHEEL_RADIUS + drop / 2), 'steel'), .002, .4))
    for side in (-1, 1):
        nut = cylinder('Nut', .02, .014, (side * (d.WHEEL_X + .052), 0, d.WHEEL_RADIUS), 'steel', SIDEWAYS, 6)
        parts.append(ready(nut, .002, .4))
    return parts
