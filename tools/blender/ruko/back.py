"""The alley face. Nobody decorates it, so it shows how the building is really used."""
from kit.finish import ready
from kit.parts import blob, box, cylinder
from kit.tubes import tube
from . import dims as d

FACING = (1.5708, 0, 0)
CLOTHES = ('cloth_red', 'cloth_white', 'cloth_blue', 'cloth_white', 'cloth_yellow')


def door_and_window():
    y = d.BACK - .04
    parts = [ready(box('BackDoorFrame', (1.0, .08, 2.15), (-1.3, y, 1.08), 'frame'), 0, 1),
             ready(box('BackDoor', (.86, .05, 2.05), (-1.3, y - .03, 1.04), 'steeldoor'), .008, 1),
             ready(box('BackWindow', (1.1, .06, .8), (1.0, y, 1.9), 'glass'), 0, 1),
             ready(box('BackWindowFrame', (1.24, .05, .94), (1.0, y + .01, 1.9), 'frame'), 0, 1)]
    for i in range(7):   # welded security bars
        x = 1.0 - .48 + i * .16
        parts.append(ready(box('Bar', (.016, .016, .86), (x, y - .07, 1.9), 'steel'), 0, 1))
    hood = box('BackHood', (1.5, .7, .025), (-1.3, y - .33, 2.42), 'awning', rot=(.25, 0, 0))
    parts.append(ready(hood, 0, 1, grain='y'))
    return parts


def utilities(floors):
    y = d.BACK - .04
    parts = [ready(box('MeterBox', (.3, .12, .42), (-.4, y - .06, 1.65), 'appliance'), .008, 1),
             ready(box('MeterGlass', (.16, .01, .12), (-.4, y - .125, 1.72), 'glass'), 0, 1),
             ready(cylinder('ExhaustFan', .2, .1, (2.0, y - .04, 2.75), 'frame', FACING, 12), 0, 1)]
    cable = [(-.4, y - .06, 1.86), (-.4, y - .06, d.roof_z(floors) - .3), (-2.2, y - .06, d.roof_z(floors) - .3)]
    parts.append(ready(tube('MeterCable', cable, .012, 'pipe', bend=.05, sides=5), 0, 1))
    for i, x in enumerate((.1, .45)):   # 3 kg LPG "melon" cylinders waiting for the refill truck
        parts.append(ready(blob('Lpg', (.27, .27, .3), (x, y - .3, .23), 'lpg', bulge=.9), 0, 1))
        parts.append(ready(cylinder('LpgCollar', .07, .1, (x, y - .3, .42), 'lpg', segments=8), 0, 1))
    parts.append(ready(cylinder('Bucket', .17, .3, (1.7, y - .4, .23), 'cloth_blue', segments=12), 0, 1))
    return parts


def upper_window(level):
    y, z = d.BACK - .04, d.floor_z(level) + 1.6
    parts = [ready(box('UpperBackWindow', (1.3, .06, 1.0), (-.9, y, z), 'glass'), 0, 1),
             ready(box('UpperBackFrame', (1.44, .05, 1.14), (-.9, y + .01, z), 'frame'), 0, 1),
             ready(box('UpperBackSill', (1.5, .14, .05), (-.9, y - .05, z - .58), 'trim'), 0, 1)]
    return parts


def laundry(level):
    """Poles pushed out of the back window with the day's washing on them."""
    y, z = d.BACK - .04, d.floor_z(level) + 2.25
    parts = []
    for x in (-.2, 2.2):
        parts.append(ready(tube('LaundryArm', [(x, y, z), (x, y - .9, z + .05)], .012, 'steel', bend=0, sides=5), 0, 1))
    parts.append(ready(tube('LaundryPole', [(-.3, y - .8, z + .05), (2.3, y - .8, z + .05)], .014, 'wood',
                            bend=0, sides=6), 0, 1))
    for i, colour in enumerate(CLOTHES):
        height = .5 + (i % 3) * .12
        cloth = box('Laundry', (.38, .012, height), (-.05 + i * .5, y - .8, z + .03 - height / 2), colour,
                    rot=(.04 * (i - 2), 0, 0))
        parts.append(ready(cloth, 0, 1))
    return parts
