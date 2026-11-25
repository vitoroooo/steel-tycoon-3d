"""The undercarriage: two steel tracks on their rollers, the frame between them and the slewing ring."""
import math
from kit.finish import ready
from kit.parts import box, cylinder
from . import dims as d

ACROSS = (0, math.pi / 2, 0)   # turns a cylinder's axis from Z to X


def track(side):
    x, radius = side * d.TRACK_GAUGE, d.TRACK_HEIGHT / 2
    straight = d.TRACK_LENGTH - d.TRACK_HEIGHT
    parts = [ready(box('Track', (d.TRACK_WIDTH, straight, d.TRACK_HEIGHT), (x, 0, radius), 'track'), .01, .4, 'y')]
    for end in (-1, 1):   # the sprocket and the idler the belt wraps round
        parts.append(ready(cylinder('TrackEnd', radius, d.TRACK_WIDTH, (x, end * straight / 2, radius), 'track', ACROSS, 20), .01, .4))
        parts.append(ready(cylinder('Sprocket', radius * .62, d.TRACK_WIDTH + .06, (x, end * straight / 2, radius), 'steel_dark', ACROSS, 14), .008))
    for k in range(6):    # bottom rollers showing in the side frame
        y = -straight / 2 + (k + .5) * straight / 6
        parts.append(ready(cylinder('Roller', .11, d.TRACK_WIDTH + .05, (x, y, .2), 'steel_dark', ACROSS, 10), .005))
    parts.append(ready(box('SideFrame', (d.TRACK_WIDTH + .04, straight * .92, .2), (x, 0, radius), 'paint_yellow'), .01))
    for k in range(18):   # grouser bars across the top run of the belt
        y = -straight / 2 + (k + .5) * straight / 18
        parts.append(ready(box('Grouser', (d.TRACK_WIDTH + .02, .07, .03), (x, y, d.TRACK_HEIGHT + .012), 'steel_dark'), .004))
    return parts


def frame():
    parts = [ready(box('CarBody', (d.TRACK_GAUGE * 2 - d.TRACK_WIDTH, 1.5, .34), (0, 0, .52), 'paint_yellow'), .02)]
    parts.append(ready(cylinder('SlewRing', .72, d.DECK - .68, (0, 0, (d.DECK + .68) / 2), 'steel_dark', segments=28), .01))
    return parts


def undercarriage():
    return track(-1) + track(1) + frame()
