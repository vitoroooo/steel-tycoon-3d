"""The 20-foot flat-deck trailer and its load of pressed scrap bales. Y is measured from the kingpin."""
from .. import shapes
from ..paint import cel
from ..rods import rod
from . import frame, spec

W, L = spec.DECK
TOP, NOSE = spec.DECK_TOP, spec.DECK_NOSE
BALES = ('rust', 'steel', 'grey')


def body(at):
    """The deck with its kingpin at Y = at."""
    back, mid = at - NOSE + L, at - NOSE + L / 2
    parts = [
        shapes.block('Dek', (0, mid, TOP - .09), (W, L, .18), cel('navy'), .2),
        shapes.block('Papan dek', (0, mid, TOP + .01), (W - .16, L - .1, .03), cel('wood'), .2),
        shapes.block('Leher', (0, at + .4, TOP - .28), (1.1, 2.6, .2), cel('navy'), .25),
        shapes.block('Palang belakang', (0, back + .02, .82), (W - .1, .12, .18), cel('navy'), .4),
        shapes.block('Pelat', (0, back + .09, .82), (.5, .02, .16), cel('white'), .3),
        shapes.block('Sandaran depan', (0, at - NOSE + .06, TOP + .55), (W, .1, 1.1), cel('yellow'), .2),
    ]
    for side in (-1, 1):
        x = side * (W / 2 - .02)
        parts += [
            shapes.block('Gelagar', (side * .55, mid + 1.0, TOP - .34), (.16, L - 2.4, .3), cel('navy'), .25),
            shapes.block('Lis dek', (x + side * .03, mid, TOP - .09), (.03, L - .2, .1), cel('yellow'), .3),
            shapes.block('Lampu belakang', (side * .9, back + .09, .82), (.3, .03, .14), cel('red'), .35),
            rod('Kaki penyangga', (side * .7, at + 1.9, TOP - .3), (side * .7, at + 1.9, .2), .06, cel('steel'), 12, .3),
            shapes.block('Tapak kaki', (side * .7, at + 1.9, .14), (.3, .3, .06), cel('steel'), .3),
        ]
        for i in range(5):
            y = at - NOSE + .6 + i * (L - 1.2) / 4
            parts.append(shapes.block('Tiang dek', (x, y, TOP + .32), (.08, .1, .64), cel('yellow'), .3))
    axles = [at + y for y in spec.DECK_AXLES]
    return parts + frame.guards(axles, True, False) + bales(at - NOSE + .9, back - .5)


def bales(front, back):
    """Two rows of pressed scrap cubes, strapped down."""
    parts, count = [], int((back - front) / 1.15)
    for i in range(count):
        y = front + (i + .5) * (back - front) / count
        for row, x in enumerate((-.56, .56)):
            layers = 2
            for layer in range(layers):
                colour = BALES[(i + row + layer) % len(BALES)]
                parts.append(shapes.block('Bal rongsok', (x, y, TOP + .46 + layer * .88), (1.04, 1.02, .86), cel(colour), .1))
        parts.append(shapes.block('Sabuk ikat', (0, y, TOP + 1.8), (W - .2, .08, .03), cel('yellow'), .3))
        for side in (-1, 1):
            parts.append(shapes.block('Sabuk ikat', (side * 1.09, y, TOP + .9), (.03, .08, 1.8), cel('yellow'), .3))
    return parts


def wheels(at):
    return frame.wheels([at + y for y in spec.DECK_AXLES], True, False)
