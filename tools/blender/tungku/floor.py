"""What stands around the furnace: the ladle on its transfer car, the operating deck and the control pulpit."""
from kit.finish import ready
from kit.members import frustum
from kit.parts import box, cylinder, ring
from kit.tubes import tube

LADLE_X = 5.6
DECK = 2.3                     # height of the operating floor behind the furnace


def ladle():
    parts = [ready(frustum('Ladle', 1.0, 1.3, 2.3, (LADLE_X, 0, 1.75), 'shell', 28), .03, 1.5, 'z'),
             ring('LadleRim', 1.27, .09, (LADLE_X, 0, 2.9), 'refractory'),
             cylinder('LadleMetal', 1.18, .05, (LADLE_X, 0, 2.82), 'melt', segments=28)]
    for y in (-1.45, 1.45):
        parts.append(cylinder('Trunnion', .16, .5, (LADLE_X, y, 2.2), 'steel_dark', rot=(1.5708, 0, 0), segments=14))
    parts.append(ready(box('Car', (3.2, 2.5, .36), (LADLE_X, 0, .42), 'steel_dark'), .03))
    for x in (-1.1, 1.1):
        for y in (-.9, .9):
            wheel = cylinder('CarWheel', .22, .14, (LADLE_X + x, y, .22), 'steel_dark', rot=(1.5708, 0, 0), segments=16)
            parts.append(wheel)
    for y in (-.9, .9):
        parts.append(ready(box('Rail', (9, .07, .1), (LADLE_X + 1.5, y, .03), 'steel_dark'), 0, 2))
    return parts


def deck():
    parts = [ready(box('Deck', (9, 3.4, .12), (-.5, -4.6, DECK), 'deck'), .01, 1.5)]
    for x in (-4.8, -.5, 3.8):
        for y in (-3.1, -6.1):
            parts.append(ready(box('DeckLeg', (.16, .16, DECK), (x, y, DECK / 2), 'steel_dark'), .008))
    for height in (.55, 1.05):
        loop = [(-5, -3, DECK + height), (-5, -6.2, DECK + height), (4, -6.2, DECK + height), (4, -3, DECK + height)]
        parts.append(tube('Handrail', loop, .025, 'rail', .1, 8))
    for x in (-5, -2, 1, 4):
        parts.append(tube('RailPost', [(x, -6.2, DECK), (x, -6.2, DECK + 1.05)], .025, 'rail', 0, 8))
    for step in range(9):      # stair down to the hall floor on the left
        z = DECK - (step + 1) * DECK / 9
        parts.append(ready(box('Tread', (.26, 1, .04), (-5.2 - step * .27, -3.6, z), 'deck'), 0, 1.5))
    for y in (-3.1, -4.1):
        parts.append(ready(box('Stringer', (2.9, .04, .22), (-6.3, y, DECK / 2 - .1), 'steel_dark', rot=(0, .76, 0)), 0))
    return parts


def pulpit():
    """The operator's cabin: a glazed box on the deck with a tilted window towards the furnace."""
    at = (-3.4, -4.9, DECK)
    parts = [ready(box('Cabin', (2.4, 2, 2.2), (at[0], at[1], at[2] + 1.16), 'cabin'), .02, 1.5),
             ready(box('CabinRoof', (2.7, 2.3, .08), (at[0], at[1], at[2] + 2.3), 'steel_dark'), .01),
             ready(box('Window', (1.9, .04, .9), (at[0], at[1] + 1.01, at[2] + 1.5), 'glass'), 0),
             ready(box('SideWindow', (.04, 1.4, .9), (at[0] + 1.21, at[1], at[2] + 1.5), 'glass'), 0),
             ready(box('Beacon', (.14, .14, .16), (at[0] + .9, at[1] + .7, at[2] + 2.42), 'melt'), .02)]
    return parts
