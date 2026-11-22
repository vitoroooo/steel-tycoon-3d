"""The cargo half: a dented steel bed on two wheels, loaded with the day's haul."""
from .. import shapes
from ..paint import cel
from ..rods import lettering, rod, wheel
from . import dims

W, L = dims.BED
Y, FLOOR, WALL = dims.BED_Y, dims.BED_FLOOR, dims.BED_WALL


def box():
    mid = FLOOR + WALL / 2
    parts = [shapes.block('Lantai bak', (0, Y, FLOOR), (W, L, .07), cel('navy'), .2),
             shapes.block('Dinding depan', (0, Y - L / 2, mid + .1), (W, .05, WALL + .2), cel('yellow'), .3),
             shapes.block('Pintu bak', (0, Y + L / 2, mid), (W, .05, WALL), cel('yellow'), .3)]
    for side in (-1, 1):
        parts.append(shapes.block('Dinding bak', (side * W / 2, Y, mid), (.05, L, WALL), cel('yellow'), .3))
        parts.append(shapes.block('Rusuk bak', (side * (W / 2 + .012), Y, FLOOR + WALL), (.065, L + .05, .05), cel('navy'), .4))
        parts.append(shapes.block('Tambalan', (side * (W / 2 + .02), Y + .56, mid - .06), (.03, .22, .15), cel('rust'), .2))
        parts += wheel((side * dims.TRACK, dims.REAR_Y, dims.WHEEL), dims.WHEEL, .15, 'yellow')
        parts.append(shapes.block('Spakbor belakang', (side * dims.TRACK, dims.REAR_Y, dims.WHEEL * 2 + .03), (.2, .5, .05), cel('navy'), .45))
        parts.append(shapes.ball('Lampu belakang', (side * .5, Y + L / 2 + .03, FLOOR + .1), (.07, .025, .05), cel('red')))
    parts += [
        rod('As belakang', (-dims.TRACK, dims.REAR_Y, dims.WHEEL), (dims.TRACK, dims.REAR_Y, dims.WHEEL), .035, cel('steel')),
        shapes.block('Sasis', (0, .3, .44), (.5, 1.4, .09), cel('navy'), .3),
        shapes.block('Pelat', (0, Y + L / 2 + .035, FLOOR + .1), (.34, .02, .12), cel('white'), .3),
        lettering('Nama usaha', 'UD. ROSOK JAYA', (W / 2 + .03, Y, mid + .04), .15, cel('navy'), 'x'),
        lettering('Nama usaha', 'UD. ROSOK JAYA', (-W / 2 - .03, Y, mid + .04), .15, cel('navy'), '-x'),
    ]
    return parts


def haul():
    top = FLOOR + WALL
    parts = [rod('Drum', (-.3, Y - .35, FLOOR + .05), (-.3, Y - .35, top + .32), .2, cel('rust'), 20, .2),
             rod('Gelang drum', (-.3, Y - .35, top + .08), (-.3, Y - .35, top + .12), .21, cel('orange'), 20, .3),
             rod('Ban bekas', (.28, Y + .4, top + .0), (.28, Y + .4, top + .13), .26, cel('tire'), 24, .5),
             rod('Lubang ban', (.28, Y + .4, top + .05), (.28, Y + .4, top + .14), .13, cel('ink'), 20, .2),
             shapes.block('Kulkas bekas', (.3, Y - .3, top + .12), (.42, .5, .62), cel('light'), .2, .1),
             shapes.block('Gagang kulkas', (.3, Y - .56, top + .2), (.2, .03, .04), cel('steel'), .4, .1)]
    for i, tone in enumerate(('steel', 'light', 'rust')):
        parts.append(rod('Pipa', (-.45 + i * .09, Y - .1, top - .05), (-.3 + i * .1, Y + .85, top + .25 + i * .05), .035, cel(tone)))
    parts.append(shapes.ball('Karung', (-.2, Y + .32, top + .05), (.3, .32, .24), cel('cream')))
    return parts
