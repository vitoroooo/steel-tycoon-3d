"""The door end: two leaves, four locking rods with cams and handles, hinges and the data plate."""
from kit.finish import ready
from kit.parts import box, cylinder
from .dims import HEIGHT, LENGTH, POST, RAIL, WIDTH

FACE = -LENGTH / 2 + .03        # outer face of the door leaves
LEAF = (WIDTH - POST * 2) / 2
TOP, BOTTOM = HEIGHT - RAIL, RAIL


def leaves():
    parts = []
    for side in (-1, 1):
        x = side * LEAF / 2
        parts.append(ready(box('Leaf', (LEAF - .015, .04, TOP - BOTTOM - .02), (x, FACE + .02, HEIGHT / 2), 'paint'),
                           .006, 1.94, 'z'))
        for z in (.55, 1.3, 2.05):   # pressed stiffening ribs
            parts.append(ready(box('Rib', (LEAF - .12, .012, .05), (x, FACE - .004, z), 'paint'), .004, 1.94))
        for z in (.35, 1.0, 1.6, 2.25):
            hinge = (side * (LEAF + .02), FACE - .005, z)
            parts.append(ready(cylinder('Hinge', .022, .14, hinge, 'steel', segments=10), 0))
    return parts


def rods():
    parts = []
    for side in (-1, 1):
        for offset, handle in ((.22, True), (.75, True)):
            x = side * offset
            parts.append(ready(cylinder('Rod', .017, TOP - BOTTOM + .1, (x, FACE - .035, HEIGHT / 2), 'steel',
                                        segments=10), 0))
            for z in (BOTTOM - .02, TOP + .02):
                parts.append(ready(box('Cam', (.07, .05, .05), (x, FACE - .035, z), 'steel'), .006))
            for z in (.5, 1.25, 2.1):
                parts.append(ready(box('Guide', (.07, .03, .035), (x, FACE - .02, z), 'steel'), .004))
            if handle:
                parts.append(ready(box('Handle', (.3, .014, .035), (x - side * .14, FACE - .05, 1.12), 'steel',
                                       rot=(0, side * .12, 0)), .004))
                parts.append(ready(box('Catch', (.06, .03, .08), (x - side * .27, FACE - .03, 1.12), 'steel'), .004))
    return parts


def plate():
    """The CSC data plate on the left leaf and the seal tag hanging from the right handle."""
    return [ready(box('Plate', (.26, .006, .18), (-LEAF / 2 - .05, FACE - .006, .8), 'plate'), .002),
            ready(box('Seal', (.03, .012, .09), (.5, FACE - .06, 1.04), 'seal'), .003)]
