"""The cab: a flat-nosed box with big windows, round lamps and a tired bumper."""
from .. import shapes
from ..paint import cel
from ..rods import rod
from . import dims

W, L = dims.CAB
Y, NOSE, BELT, ROOF = dims.CAB_Y, dims.NOSE_Y, dims.BELT, dims.ROOF


def shell():
    glass_z, glass_h = (BELT + ROOF) / 2 - .03, ROOF - BELT - .16
    parts = [
        shapes.block('Kabin bawah', (0, Y, (BELT + .56) / 2), (W, L, BELT - .56), cel('orange'), .22),
        shapes.block('Kabin atas', (0, Y + .04, (BELT + ROOF) / 2), (W - .1, L - .12, ROOF - BELT + .1), cel('orange'), .3),
        shapes.block('Atap', (0, Y + .04, ROOF + .02), (W - .2, L - .22, .07), cel('cream'), .45),
        shapes.block('Kaca depan', (0, NOSE + .075, glass_z), (W - .34, .04, glass_h), cel('glass'), .3),
        shapes.block('Kaca belakang', (0, Y + L / 2 - .03, glass_z), (W - .6, .04, glass_h - .08), cel('glass'), .3),
        shapes.block('Strip depan', (0, NOSE - .012, BELT - .14), (W - .3, .04, .09), cel('cream'), .4),
        shapes.block('Gril', (0, NOSE - .01, .92), (.62, .03, .16), cel('navy'), .3),
        shapes.block('Bumper depan', (0, NOSE - .05, .62), (W + .06, .16, .17), cel('navy'), .45),
        shapes.block('Pelat depan', (0, NOSE - .135, .62), (.36, .02, .12), cel('white'), .3),
        shapes.block('Karat pintu', (W / 2 + .004, Y + .3, .72), (.02, .34, .16), cel('rust'), .2),
        rod('Terpal', (-.5, Y + .05, ROOF + .12), (.5, Y + .05, ROOF + .12), .09, cel('teal'), 14, .6),
    ]
    for i in range(3):
        parts.append(shapes.block('Kisi gril', (0, NOSE - .028, .87 + i * .05), (.54, .012, .014), cel('steel'), .3))
    return parts + _lamps() + _sides(glass_z, glass_h)


def _lamps():
    parts = []
    for side in (-1, 1):
        x = side * .56
        parts += [
            rod('Lampu', (x, NOSE + .01, .93), (x, NOSE - .045, .93), .115, cel('cream'), 20, .3),
            rod('Ring lampu', (x, NOSE + .02, .93), (x, NOSE - .02, .93), .135, cel('navy'), 20, .3),
            shapes.ball('Sein', (side * .74, NOSE - .02, .78), (.06, .03, .04), cel('yellow')),
            rod('Wiper', (side * .1, NOSE + .05, BELT + .05), (side * .44, NOSE + .05, BELT + .3), .012, cel('ink'), 8),
            rod('Tali terpal', (side * .32, dims.CAB_Y + .05, ROOF + .04), (side * .32, dims.CAB_Y + .05, ROOF + .22),
                .1, cel('navy'), 12, .3),
        ]
    return parts


def _sides(glass_z, glass_h):
    parts = []
    for side in (-1, 1):
        x = side * (W / 2 - .045)
        parts += [
            shapes.block('Kaca samping', (x, Y + .02, glass_z), (.04, L - .5, glass_h), cel('glass'), .3),
            shapes.block('Strip samping', (side * (W / 2 + .012), Y, BELT - .14), (.04, L - .3, .09), cel('cream'), .4),
            shapes.block('Gagang pintu', (side * (W / 2 + .012), Y + .3, BELT - .28), (.025, .13, .035), cel('steel'), .4),
            shapes.block('Garis pintu', (side * (W / 2 + .001), Y + .46, 1.0), (.012, .014, .62), cel('ink'), .2),
            rod('Tangkai spion', (side * (W / 2 - .02), NOSE + .16, BELT + .12), (side * (W / 2 + .14), NOSE + .1, BELT + .2),
                .014, cel('navy'), 8),
            shapes.ball('Spion', (side * (W / 2 + .16), NOSE + .09, BELT + .26), (.03, .06, .1), cel('navy')),
            shapes.block('Spakbor depan', (side * dims.TRACK, dims.FRONT_Y, dims.WHEEL * 2 + .05), (dims.TYRE + .08, .86, .06),
                         cel('navy'), .45),
        ]
    return parts
