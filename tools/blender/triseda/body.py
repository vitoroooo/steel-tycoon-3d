"""The motorcycle half that stays put: frame, tank, saddle, engine and exhaust."""
import math
from kit.finish import ready
from kit.parts import blob, box, cylinder
from kit.tubes import tube
from . import dims as d

SIDEWAYS = (0, math.pi / 2, 0)
ENGINE_Y, ENGINE_Z = .7, .36


def frame():
    head = (0, d.HEAD_Y - .02, d.HEAD_Z - .08)
    parts = [ready(tube('Backbone', [head, (0, .6, .74), (0, .12, .66)], .024, 'frame', bend=.12), 0, .4)]
    for x in (-.08, .08):
        cradle = [head, (x, .98, .3), (x, .42, .24), (x, .16, .5)]
        parts.append(ready(tube('Cradle', cradle, .016, 'frame', bend=.1), 0, .4))
    parts.append(ready(cylinder('HeadTube', .034, .2, (0, d.HEAD_Y + .012, d.HEAD_Z - .1), 'frame', (-.47, 0, 0), 16), .003, .3))
    peg = [(-.3, ENGINE_Y - .1, .27), (.3, ENGINE_Y - .1, .27)]
    parts.append(ready(tube('PegBar', peg, .012, 'steel', bend=0), 0, .3))
    for x in (-.27, .27):
        parts.append(ready(cylinder('Peg', .02, .1, (x, ENGINE_Y - .1, .27), 'rubber', SIDEWAYS, 12), .003, .2))
    return parts


def tank_and_seat():
    parts = [ready(blob('Tank', (.3, .48, .23), (0, .8, .87), 'paint', bulge=.55, taper=.25), 0, .5)]
    parts.append(ready(cylinder('FuelCap', .035, .02, (0, .88, .985), 'chrome', segments=18), .004, .2))
    parts.append(ready(blob('Saddle', (.33, .52, .11), (0, .34, d.SEAT_Z + .03), 'seat', bulge=.45, lumps=.01), 0, .3))
    parts.append(ready(box('SeatPan', (.3, .5, .03), (0, .34, d.SEAT_Z - .03), 'frame'), .008, .3))
    for x in (-.15, .15):   # side covers under the saddle
        parts.append(ready(box('SideCover', (.025, .34, .2), (x, .36, .64), 'paint', (0, 0, 0)), .012, .5))
    return parts


def engine():
    y, z = ENGINE_Y, ENGINE_Z
    parts = [ready(cylinder('Crankcase', .115, .2, (0, y, z), 'engine', SIDEWAYS, 28), .012, .3)]
    parts.append(ready(cylinder('ClutchCover', .095, .05, (.115, y - .03, z), 'engine', SIDEWAYS, 24), .012, .3))
    parts.append(ready(cylinder('Magneto', .08, .05, (-.115, y + .02, z), 'engine', SIDEWAYS, 24), .012, .3))
    parts.append(ready(box('Gearbox', (.2, .22, .16), (0, y - .16, z - .01), 'engine'), .02, .3))
    lean = -.5   # the cylinder leans forward
    for index in range(9):   # cooling fins
        along = .1 + index * .019
        at = (0, y + .06 + math.sin(-lean) * along, z + math.cos(lean) * along)
        size = (.15, .15, .007) if index < 7 else (.13, .16, .02)
        parts.append(ready(box('Fin', size, at, 'engine', (lean, 0, 0)), .002, .3))
    parts.append(ready(cylinder('Carb', .028, .09, (0, y - .02, z + .2), 'engine', (math.pi / 2 - .3, 0, 0), 12), .004, .2))
    parts.append(ready(cylinder('AirFilter', .05, .08, (0, y - .12, z + .22), 'frame', (math.pi / 2, 0, 0), 16), .008, .2))
    return parts


def exhaust():
    y, z = ENGINE_Y, ENGINE_Z
    pipe = [(.03, y + .19, z + .2), (.06, y + .3, z + .12), (.17, y + .26, z - .12), (.2, .1, .22), (.22, -.2, .3)]
    parts = [ready(tube('Header', pipe, .018, 'steel', bend=.08), 0, .3)]
    parts.append(ready(cylinder('Muffler', .048, .52, (.22, -.46, .3), 'steel', (math.pi / 2, 0, 0), 18), .012, .3))
    parts.append(ready(cylinder('Tailpipe', .016, .08, (.22, -.75, .3), 'frame', (math.pi / 2, 0, 0), 10), .002, .2))
    parts.append(ready(box('HeatShield', (.012, .3, .06), (.275, -.42, .31), 'chrome'), .004, .3))
    return parts
