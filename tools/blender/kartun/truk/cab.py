"""The cab every heavy truck shares: tall, flat-nosed, with a visor, a beacon and steps to climb in."""
from .. import shapes
from ..paint import cel
from ..rods import rod
from . import spec

W, L = spec.CAB
BELT, ROOF, FLOOR = spec.BELT, spec.ROOF, .72


def shell(truck):
    y, nose = spec.cab_y(truck), spec.nose(truck)
    glass_z, glass_h = (BELT + ROOF) / 2 - .04, ROOF - BELT - .3
    parts = [
        shapes.block('Kabin bawah', (0, y, (BELT + FLOOR) / 2), (W, L, BELT - FLOOR), cel('orange'), .16),
        shapes.block('Kabin atas', (0, y + .05, (BELT + ROOF) / 2), (W - .12, L - .14, ROOF - BELT + .12), cel('orange'), .26),
        shapes.block('Atap', (0, y + .05, ROOF + .03), (W - .26, L - .28, .09), cel('cream'), .45),
        shapes.block('Pelindung matahari', (0, nose + .02, ROOF - .12), (W - .3, .24, .07), cel('navy'), .4),
        shapes.block('Kaca depan', (0, nose + .09, glass_z), (W - .42, .05, glass_h), cel('glass'), .3),
        shapes.block('Strip depan', (0, nose - .012, BELT - .17), (W - .3, .05, .12), cel('cream'), .4),
        shapes.block('Gril', (0, nose - .012, 1.22), (1.25, .04, .44), cel('navy'), .2),
        shapes.block('Bumper', (0, nose - .08, .7), (W + .1, .24, .28), cel('navy'), .4),
        shapes.block('Pelat depan', (0, nose - .21, .7), (.5, .02, .16), cel('white'), .3),
        rod('Lampu rotator', (0, y, ROOF + .08), (0, y, ROOF + .24), .1, cel('yellow'), 14, .5),
    ]
    for i in range(4):
        parts.append(shapes.block('Kisi gril', (0, nose - .035, 1.07 + i * .1), (1.12, .014, .022), cel('steel'), .3))
    return parts + _front(nose) + _sides(y, nose, glass_z, glass_h)


def _front(nose):
    parts = []
    for side in (-1, 1):
        x = side * .82
        parts += [
            rod('Lampu', (x, nose - .1, .74), (x, nose - .215, .74), .11, cel('cream'), 20, .3),
            shapes.ball('Sein', (side * 1.06, nose - .19, .74), (.07, .03, .06), cel('yellow')),
            rod('Wiper', (side * .12, nose + .06, BELT + .06), (side * .6, nose + .06, BELT + .42), .014, cel('ink'), 8),
            rod('Lampu atap', (side * .6, nose + .1, ROOF + .09), (side * .6, nose + .04, ROOF + .09), .05, cel('yellow'), 10, .4),
        ]
    return parts


def _sides(y, nose, glass_z, glass_h):
    parts = []
    for side in (-1, 1):
        edge = side * W / 2
        parts += [
            shapes.block('Kaca samping', (edge - side * .05, y + .02, glass_z), (.05, L - .6, glass_h), cel('glass'), .3),
            shapes.block('Strip samping', (edge + side * .012, y, BELT - .17), (.05, L - .3, .12), cel('cream'), .4),
            shapes.block('Gagang pintu', (edge + side * .014, y + .5, BELT - .4), (.03, .16, .04), cel('steel'), .4),
            shapes.block('Garis pintu', (edge + side * .002, y + .7, 1.25), (.012, .016, .95), cel('ink'), .2),
            shapes.block('Pijakan', (edge - side * .06, y + .25, .58), (.2, .5, .06), cel('steel'), .3),
            rod('Tangkai spion', (edge, nose + .2, BELT + .2), (edge + side * .22, nose + .08, BELT + .3), .018, cel('navy'), 8),
            shapes.block('Spion', (edge + side * .24, nose + .06, BELT + .5), (.05, .09, .42), cel('navy'), .4),
        ]
    return parts
