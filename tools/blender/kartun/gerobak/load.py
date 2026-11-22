"""What rides in the cart: sacks, a few finds, the hanging scale and the hand-painted sign."""
from .. import shapes
from ..paint import cel
from ..rods import lettering, rod
from .box import FLOOR, LENGTH, WALL, WIDTH

TOP = FLOOR + WALL


def sacks():
    parts = []
    for x, y, z, size, tone in ((-.17, -.25, TOP - .02, .27, 'cream'), (.2, -.05, TOP - .06, .24, 'light'),
                                (-.1, .28, TOP - .08, .22, 'cream')):
        parts.append(shapes.ball('Karung', (x, y, z), (size, size * 1.05, size * .82), cel(tone)))
        parts.append(shapes.capsule('Ikatan', (x, y, z + size * .7), (x + .03, y, z + size * 1.08), size * .2, size * .28, cel(tone), 12))
        parts.append(rod('Tali', (x, y, z + size * .78), (x, y, z + size * .86), size * .23, cel('orange'), 12, .5))
    return parts


def finds():
    return [
        rod('Drum', (.24, .36, TOP - .12), (.24, .36, TOP + .22), .15, cel('rust'), 20, .2),
        rod('Gelang drum', (.24, .36, TOP + .08), (.24, .36, TOP + .11), .158, cel('orange'), 20, .3),
        rod('Pipa', (.36, -.5, TOP - .1), (.3, .1, TOP + .3), .035, cel('steel')),
        rod('Pipa', (.3, -.52, TOP - .1), (.2, .0, TOP + .34), .028, cel('light')),
        shapes.block('Seng', (-.3, .05, TOP + .05), (.04, .6, .34), cel('steel'), .2, .12),
    ]


def scale():
    """A spring scale on a hooked pole: how every purchase is weighed."""
    x, y = -WIDTH / 2, LENGTH / 2
    return [
        rod('Tiang timbangan', (x, y, FLOOR), (x, y, TOP + .62), .024, cel('navy')),
        rod('Lengan', (x, y, TOP + .6), (x - .26, y, TOP + .6), .02, cel('navy')),
        rod('Timbangan', (x - .24, y, TOP + .52), (x - .24, y, TOP + .3), .06, cel('yellow'), 16, .35),
        shapes.ball('Muka timbangan', (x - .24, y - .055, TOP + .42), (.04, .014, .055), cel('white')),
        rod('Kait', (x - .24, y, TOP + .3), (x - .24, y, TOP + .2), .012, cel('steel'), 8),
        shapes.tube('Ujung kait', shapes.arc((x - .24, y, TOP + .17), .035, 90, 330, 'xz', 7), .012, cel('steel')),
    ]


def sign():
    y = -LENGTH / 2 - .04
    return [
        shapes.block('Papan nama', (0, y, FLOOR + .3), (.78, .035, .3), cel('yellow'), .3),
        lettering('Tulisan', 'TERIMA', (0, y - .02, FLOOR + .365), .13, cel('navy')),
        lettering('Tulisan', 'ROSOK', (0, y - .02, FLOOR + .23), .16, cel('red')),
    ]
