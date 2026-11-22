"""The cargo half: a ribbed steel bed with a drop tailgate, on a ladder chassis."""
from .. import shapes
from ..paint import cel
from ..rods import lettering, rod, wheel
from . import dims

W, L = dims.BED
Y, FLOOR, WALL = dims.BED_Y, dims.BED_FLOOR, dims.BED_WALL
BACK = Y + L / 2


def box():
    mid = FLOOR + WALL / 2
    parts = [
        shapes.block('Lantai bak', (0, Y, FLOOR), (W, L, .08), cel('navy'), .2),
        shapes.block('Dinding depan', (0, Y - L / 2, mid + .12), (W, .06, WALL + .24), cel('light'), .3),
        shapes.block('Pintu bak', (0, BACK, mid), (W, .06, WALL), cel('light'), .3),
        shapes.block('Sasis', (0, -.05, .52), (.9, 3.7, .12), cel('navy'), .3),
        shapes.block('Bumper belakang', (0, BACK + .08, .6), (W - .1, .1, .12), cel('navy'), .45),
        shapes.block('Pelat belakang', (0, BACK + .04, FLOOR + .12), (.36, .02, .12), cel('white'), .3),
        rod('Knalpot', (.5, BACK - .5, .45), (.5, BACK + .12, .45), .04, cel('steel'), 12, .4),
        shapes.block('Terpal lipat', (-.45, Y - L / 2 + .3, FLOOR + .13), (.6, .42, .18), cel('orange'), .4, .08),
        shapes.tube('Tambang', shapes.arc((.45, Y - L / 2 + .35, FLOOR + .08), .17, 0, 360, 'xy', 14), .035, cel('wood'), True),
    ]
    for side in (-1, 1):
        x = side * W / 2
        parts += [
            shapes.block('Dinding bak', (x, Y, mid), (.06, L, WALL), cel('light'), .3),
            shapes.block('Rusuk bak', (side * (W / 2 + .012), Y, FLOOR + WALL), (.08, L + .06, .06), cel('navy'), .4),
            shapes.block('Tambalan', (side * (W / 2 + .025), Y + .7, mid - .05), (.03, .3, .18), cel('rust'), .2),
            shapes.block('Lampu belakang', (side * (W / 2 - .12), BACK + .035, FLOOR + .13), (.14, .03, .2), cel('red'), .35),
            shapes.block('Spakbor belakang', (side * dims.TRACK, dims.REAR_Y, dims.WHEEL * 2 + .02),
                         (dims.TYRE + .08, .86, .05), cel('navy'), .45),
            shapes.block('Lumpur', (side * dims.TRACK, dims.REAR_Y + .46, dims.WHEEL + .1), (dims.TYRE, .02, .3), cel('ink'), .2),
            lettering('Nama usaha', 'UD. ROSOK JAYA', (side * (W / 2 + .035), Y - .1, mid + .02), .2, cel('navy'),
                      'x' if side > 0 else '-x'),
        ]
        for i in range(4):
            parts.append(shapes.block('Tiang bak', (side * (W / 2 + .02), Y - L / 2 + .2 + i * (L - .4) / 3, mid),
                                      (.05, .07, WALL), cel('navy'), .3))
    return parts


def wheels():
    """Front left, front right, rear left, rear right; +X is the driver's left."""
    return [wheel((side * dims.TRACK, y, dims.WHEEL), dims.WHEEL, dims.TYRE, 'cream')
            for y in (dims.FRONT_Y, dims.REAR_Y) for side in (1, -1)]
