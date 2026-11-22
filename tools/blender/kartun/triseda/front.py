"""The motorbike half: fork, bars, lamp, tank, saddle and engine."""
from .. import shapes
from ..paint import cel
from ..rods import rod, wheel
from . import dims

BAR_Y, BAR_Z = dims.BAR


def steering():
    parts = wheel((0, dims.FRONT_Y, dims.WHEEL), dims.WHEEL, .13, 'yellow')
    for side in (-1, 1):
        parts.append(rod('Garpu', (side * .1, dims.FRONT_Y, dims.WHEEL), (side * .1, BAR_Y + .02, BAR_Z - .12), .03, cel('steel')))
        parts.append(rod('Grip', (side * .25, BAR_Y, BAR_Z + .03), (side * .4, BAR_Y + .05, BAR_Z + .03), .036, cel('navy'), 12, .5))
        parts.append(rod('Tangkai spion', (side * .2, BAR_Y, BAR_Z), (side * .3, BAR_Y - .02, BAR_Z + .24), .012, cel('navy'), 8))
        parts.append(shapes.ball('Spion', (side * .31, BAR_Y - .02, BAR_Z + .28), (.075, .02, .055), cel('navy')))
    parts += [
        rod('Setang', (-.27, BAR_Y, BAR_Z + .03), (.27, BAR_Y, BAR_Z + .03), .022, cel('steel')),
        shapes.block('Batok', (0, BAR_Y - .05, BAR_Z - .1), (.3, .2, .3), cel('teal'), .45),
        rod('Lampu', (0, BAR_Y - .13, BAR_Z - .08), (0, BAR_Y - .2, BAR_Z - .08), .1, cel('cream'), 20, .3),
        rod('Ring lampu', (0, BAR_Y - .12, BAR_Z - .08), (0, BAR_Y - .17, BAR_Z - .08), .115, cel('navy'), 20, .3),
        shapes.tube('Spakbor', shapes.arc((0, dims.FRONT_Y, dims.WHEEL), dims.WHEEL + .06, 20, 150, 'yz', 9), .05, cel('teal')),
        shapes.block('Pelat depan', (0, BAR_Y - .16, BAR_Z - .3), (.24, .02, .09), cel('white'), .3),
    ]
    return parts


def frame():
    return [
        shapes.ball('Tangki', (0, -.5, .88), (.2, .3, .16), cel('teal')),
        rod('Tutup tangki', (0, -.56, 1.02), (0, -.56, 1.06), .045, cel('yellow'), 12, .3),
        shapes.block('Strip tangki', (0, -.5, .9), (.415, .34, .045), cel('yellow'), .45),
        shapes.block('Jok', (0, -.1, dims.SEAT_Z), (.34, .56, .13), cel('navy'), .45),
        shapes.block('Mesin', (0, -.48, .5), (.3, .36, .3), cel('grey'), .3),
        shapes.block('Sirip mesin', (0, -.52, .52), (.34, .2, .2), cel('steel'), .2),
        rod('Rangka', (0, BAR_Y + .05, BAR_Z - .2), (0, -.2, .42), .04, cel('navy')),
        rod('Rangka bawah', (0, -.75, .38), (0, .1, .42), .04, cel('navy')),
        rod('Knalpot', (.2, -.45, .4), (.24, .25, .36), .05, cel('steel'), 14, .5),
        rod('Ujung knalpot', (.24, .2, .36), (.245, .36, .355), .062, cel('light'), 14, .4),
        shapes.block('Pijakan', (0, -.28, .33), (.62, .1, .035), cel('navy'), .4),
    ]
