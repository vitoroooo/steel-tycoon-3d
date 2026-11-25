"""The cart's body: patched plank walls on a steel frame, with push handles and a prop leg."""
from .. import shapes
from ..paint import cel
from ..rods import rod, wheel

WIDTH, LENGTH, FLOOR, WALL = .92, 1.3, .44, .5
PLANKS = ('teal', 'teal', 'cream', 'teal')      # one board was replaced with whatever was around


def walls():
    parts = [shapes.block('Lantai', (0, 0, FLOOR), (WIDTH, LENGTH, .07), cel('wood'), .2)]
    height = WALL / len(PLANKS)
    for i, tone in enumerate(PLANKS):
        z = FLOOR + .05 + height * (i + .5)
        for side in (-1, 1):
            shade = 'yellow' if (side, i) == (1, 1) else tone
            parts.append(shapes.block('Papan sisi', (side * WIDTH / 2, 0, z), (.05, LENGTH, height - .012), cel(shade), .3))
            parts.append(shapes.block('Papan ujung', (0, side * LENGTH / 2, z), (WIDTH, .05, height - .012),
                                      cel('teal' if i != 2 else 'cream'), .3))
    for x in (-1, 1):
        for y in (-1, 1):
            parts.append(shapes.block('Tiang sudut', (x * WIDTH / 2, y * LENGTH / 2, FLOOR + WALL / 2 + .04),
                                      (.085, .085, WALL + .1), cel('navy'), .3))
    return parts


def running_gear():
    parts = []
    for side in (-1, 1):
        parts += wheel((side * (WIDTH / 2 + .1), -.08, .33), .33, .09, 'yellow', 4)
        grip_end = (side * .33, LENGTH / 2 + .62, FLOOR + WALL + .12)
        parts.append(rod('Gagang', (side * .4, LENGTH / 2 - .1, FLOOR + .2), grip_end, .028, cel('navy')))
        parts.append(rod('Pegangan', (side * .345, LENGTH / 2 + .42, FLOOR + WALL + .065), grip_end, .04, cel('orange')))
        parts.append(rod('Kaki', (side * .3, LENGTH / 2 - .12, FLOOR), (side * .3, LENGTH / 2 - .04, .03), .026, cel('navy')))
    parts.append(rod('As', (-WIDTH / 2 - .1, -.08, .33), (WIDTH / 2 + .1, -.08, .33), .03, cel('steel')))
    parts.append(rod('Palang', (-.345, LENGTH / 2 + .5, FLOOR + WALL + .085), (.345, LENGTH / 2 + .5, FLOOR + WALL + .085),
                     .024, cel('navy')))
    return parts
