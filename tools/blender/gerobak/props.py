"""The things a pengepul hangs on the cart: sack, hanging scale and the hand-painted sign."""
import math
from kit.finish import ready
from kit.parts import blob, box, cylinder, ring
from kit.tubes import text, tube
from . import dims as d

TOP = d.FLOOR + d.WALL


def sack():
    """An empty karung folded over the left wall, tied with a strip of rope."""
    x = -d.HALF_WIDTH - .035
    outer = blob('Sack', (.07, .5, .56), (x, .22, TOP - .23), 'sack', .25, lumps=.09, taper=.35)
    fold = blob('SackFold', (.17, .47, .075), (x + .06, .22, TOP + .03), 'sack', .5, lumps=.12)
    inner = blob('SackInner', (.05, .44, .2), (x + .13, .22, TOP - .08), 'sack', .3, lumps=.1, taper=.5)
    rope = [(x - .045, .2, TOP - .12), (x - .045, .2, TOP + .075), (x + .165, .2, TOP + .075),
            (x + .165, .2, TOP - .06)]
    cloth = [ready(part, 0, .35) for part in (outer, fold, inner)]
    return cloth + [ready(tube('Rope', rope, .006, 'rope', bend=.03, sides=6), 0, .1)]


def scale():
    """Timbangan gantung: the hanging spring scale used to weigh each purchase."""
    x, y = d.HALF_WIDTH + .045, -d.HALF_LENGTH + .12
    hook = [(x - .04, y, TOP + .01), (x, y, TOP + .01), (x, y, TOP - .05)]
    parts = [ready(tube('ScaleHanger', hook, .004, 'spoke', bend=.02, sides=6), 0, .3)]
    parts.append(ready(cylinder('ScaleBody', .028, .2, (x, y, TOP - .15), 'brass', segments=18), .003, .3))
    parts.append(ready(box('ScaleFace', (.004, .03, .15), (x + .027, y, TOP - .15), 'paint'), .001, .3))
    low = [(x, y, TOP - .25), (x, y, TOP - .33), (x + .035, y, TOP - .36), (x + .035, y, TOP - .31)]
    parts.append(ready(tube('ScaleHook', low, .0045, 'spoke', bend=.025, sides=6), 0, .3))
    parts.append(ready(ring('ScaleRing', .018, .004, (x, y, TOP - .04), 'spoke', (math.pi / 2, 0, 0), 1, 16), 0, .3))
    return parts


def sign():
    """A plywood board on the nose with hand-painted lettering."""
    y = d.HALF_LENGTH + d.BAR / 2 + .012
    board = ready(box('SignBoard', (.52, .012, .25), (0, y, d.FLOOR + .25), 'signboard'), .003, .6, 'z')
    words = text('SignText', 'TERIMA\nROSOK', .082, (0, y + .0065, d.FLOOR + .25), 'paint',
                 (math.pi / 2, 0, math.pi))
    parts = [board, ready(words, 0, .3)]
    for x in (-.23, .23):
        for z in (-.1, .1):
            bolt = cylinder('Bolt', .007, .008, (x, y + .008, d.FLOOR + .25 + z), 'steel', (math.pi / 2, 0, 0), 6)
            parts.append(ready(bolt, .001, .3))
    return parts
