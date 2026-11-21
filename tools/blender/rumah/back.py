"""The yard face: a seng-roofed kitchen lean-to, the back door, a water drum, firewood and a washing line."""
import math
from kit.finish import ready
from kit.parts import box, cylinder
from kit.tubes import tube
from . import dims as d, roof

Y = d.BACK
CLOTHES = ('cloth_white', 'cloth_red', 'cloth_blue', 'cloth_yellow', 'cloth_white', 'cloth_blue')


def kitchen():
    """Half-width lean-to with plank walls up to waist height and open above for the smoke."""
    x, half = -1.4, 1.5
    top = d.PLINTH + 2.35
    parts = [roof.lean_to('KitchenRoof', Y, -(d.KITCHEN + .3), half + .2, x, 'seng', top),
             ready(box('KitchenFloor', (half * 2, d.KITCHEN, .1), (x, Y - d.KITCHEN / 2, .05), 'concrete'), 0, 2),
             ready(box('KitchenWall', (half * 2, .04, 1.0), (x, Y - d.KITCHEN, .6), 'planks'), 0, 1),
             ready(box('KitchenSide', (.04, d.KITCHEN, 1.0), (x - half, Y - d.KITCHEN / 2, .6), 'planks'), 0, 1)]
    for px in (x - half, x + half):
        parts.append(ready(box('KitchenPost', (.08, .08, 2.0), (px, Y - d.KITCHEN, 1.0), 'wood'), 0, 1))
    parts.append(ready(box('Stove', (.9, .5, .7), (x - .8, Y - .35, .45), 'concrete'), .01, 1))
    parts.append(ready(cylinder('Wok', .22, .08, (x - .8, Y - .35, .84), 'steel', segments=12), 0, 1))
    return parts


def door():
    x = 1.6
    return [ready(box('BackDoorFrame', (.96, .08, 2.1), (x, Y - .01, d.PLINTH + 1.05), 'frame'), 0, 1),
            ready(box('BackDoor', (.82, .05, 2.0), (x, Y - .04, d.PLINTH + 1.0), 'planks'), .006, 1, grain='z'),
            ready(box('BackStep', (1.0, .4, d.PLINTH / 2), (x, Y - .25, d.PLINTH / 4), 'concrete'), 0, 2)]


def yard():
    parts = [ready(cylinder('Drum', .29, .9, (2.5, Y - .5, .45), 'drum', segments=16), .01, 1),
             ready(cylinder('DrumLid', .3, .03, (2.5, Y - .5, .915), 'drum', segments=16), 0, 1),
             ready(cylinder('Dipper', .08, .1, (2.5, Y - .5, .98), 'cloth_red', segments=8), 0, 1)]
    for row in range(3):   # split firewood stacked against the wall
        for i in range(5 - row):
            log = cylinder('Firewood', .06, .5, (.35 + i * .13 + row * .065, Y - .3, .07 + row * .11), 'wood',
                           (math.pi / 2, 0, 0), 6)
            parts.append(ready(log, 0, .5))
    return parts


LINE_Y, LINE_Z = Y - d.KITCHEN - .9, 1.7


def washing_line():
    parts = []
    for x in (-d.HALF + .3, d.HALF - .3):
        parts.append(ready(tube('LinePost', [(x, LINE_Y, 0), (x, LINE_Y, LINE_Z + .1)], .025, 'bamboo', bend=0, sides=6), 0, .5))
    parts.append(ready(tube('Line', [(-d.HALF + .3, LINE_Y, LINE_Z), (d.HALF - .3, LINE_Y, LINE_Z)], .005, 'steel',
                            bend=0, sides=4), 0, 1))
    return parts


def washing():
    """The clothes on the line, kept apart from the house so the wind can move them."""
    parts = []
    for i, colour in enumerate(CLOTHES):
        height = .55 + (i % 3) * .15
        parts.append(ready(box('Washing', (.5, .012, height), (-2.0 + i * .8, LINE_Y, LINE_Z - height / 2), colour,
                               rot=(.05 * (i % 3 - 1), 0, 0)), 0, 1))
    return parts
