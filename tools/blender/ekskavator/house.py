"""The revolving house: engine cover, counterweight, the operator's cab and its glass."""
from kit.finish import ready
from kit.parts import blob, box, cylinder
from kit.tubes import tube
from . import dims as d

CAB_WIDTH, CAB_LENGTH, CAB_HEIGHT = .95, 1.45, 1.55
CAB_X, CAB_Y = -d.HOUSE_WIDTH / 2 + CAB_WIDTH / 2 + .05, .75


def body():
    mid = d.DECK + d.HOUSE_HEIGHT / 2
    parts = [ready(box('Deck', (d.HOUSE_WIDTH, d.HOUSE_LENGTH, .16), (0, -.35, d.DECK + .08), 'steel_dark'), .01)]
    parts.append(ready(box('Engine', (d.HOUSE_WIDTH - .1, 1.9, d.HOUSE_HEIGHT), (0, -.85, mid + .1), 'paint_yellow'), .04))
    parts.append(ready(blob('Counterweight', (d.HOUSE_WIDTH, .75, d.HOUSE_HEIGHT + .05), (0, -2.05, mid + .05), 'steel_dark', bulge=.18), 0, 1))
    for k in range(5):   # cooling louvres down the right flank
        parts.append(ready(box('Louvre', (.02, .9, .05), (d.HOUSE_WIDTH / 2 - .04, -.95, d.DECK + .4 + k * .13), 'steel_dark'), .004))
    parts.append(ready(box('Tank', (.85, 1.2, .7), (d.HOUSE_WIDTH / 2 - .5, .55, d.DECK + .5), 'paint_yellow'), .04))
    parts.append(ready(cylinder('Exhaust', .05, .6, (.55, -1.3, d.DECK + d.HOUSE_HEIGHT + .4), 'steel_dark', segments=10), .004))
    rail = [(d.HOUSE_WIDTH / 2 - .06, -.2, d.DECK + d.HOUSE_HEIGHT + .1), (d.HOUSE_WIDTH / 2 - .06, -.2, d.DECK + d.HOUSE_HEIGHT + .6),
            (d.HOUSE_WIDTH / 2 - .06, -1.7, d.DECK + d.HOUSE_HEIGHT + .6), (d.HOUSE_WIDTH / 2 - .06, -1.7, d.DECK + d.HOUSE_HEIGHT + .1)]
    parts.append(tube('Handrail', rail, .018, 'steel_dark', bend=.08, sides=6))
    return parts


def cab():
    z = d.DECK + .16 + CAB_HEIGHT / 2
    parts = [ready(box('Cab', (CAB_WIDTH, CAB_LENGTH, CAB_HEIGHT), (CAB_X, CAB_Y, z), 'paint_yellow'), .05)]
    glass = z + .22
    parts.append(ready(box('Windscreen', (CAB_WIDTH - .14, .02, CAB_HEIGHT - .5), (CAB_X, CAB_Y + CAB_LENGTH / 2 + .005, glass), 'glass'), .004))
    for side in (-1, 1):
        parts.append(ready(box('SideGlass', (.02, CAB_LENGTH - .3, CAB_HEIGHT - .75),
                               (CAB_X + side * (CAB_WIDTH / 2 + .005), CAB_Y, glass + .1), 'glass'), .004))
    parts.append(ready(box('RoofCap', (CAB_WIDTH + .06, CAB_LENGTH + .1, .05), (CAB_X, CAB_Y, z + CAB_HEIGHT / 2 + .02), 'steel_dark'), .01))
    parts.append(ready(cylinder('Beacon', .05, .09, (CAB_X, CAB_Y - .4, z + CAB_HEIGHT / 2 + .09), 'beacon', segments=10), .004))
    parts.append(ready(box('WorkLamp', (.16, .06, .1), (CAB_X, CAB_Y + CAB_LENGTH / 2 + .02, z + CAB_HEIGHT / 2 - .08), 'lamp'), .006))
    return parts
