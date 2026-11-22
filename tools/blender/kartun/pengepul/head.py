"""The head everyone in Sukabaja shares (bean skull, dot eyes, lopsided grin) and the pengepul's hair."""
import math
from .. import shapes
from ..paint import cel
from . import dims

X, Y, Z = dims.HEAD


def skull(ears=True):
    parts = [shapes.ball('Kepala', dims.HEAD, dims.SKULL, cel('skin'), 32, True)]
    for side in (-1, 1) if ears else ():
        parts.append(shapes.ball('Telinga', (side * .325, .02, Z - .03), (.05, .035, .07), cel('skin')))
        parts.append(shapes.ball('Daun telinga', (side * .335, .0, Z - .03), (.022, .02, .04), cel('skin_dark')))
    return parts


def face():
    parts = []
    for side in (-1, 1):
        parts.append(shapes.ball('Mata', (side * .125, dims.FACE_Y + .012, Z - .02), (.034, .013, .044), cel('ink')))
        parts.append(shapes.ball('Kilau', (side * .125 + .012, dims.FACE_Y - .006, Z - .002), (.011, .008, .011), cel('white')))
        parts.append(shapes.ball('Pipi', (side * .215, dims.FACE_Y + .06, Z - .095), (.05, .02, .032), cel('blush')))
        brow = [(side * .08, dims.FACE_Y - .012, Z + .062), (side * .13, dims.FACE_Y - .008, Z + .078),
                (side * .18, dims.FACE_Y + .012, Z + .066)]
        parts.append(shapes.tube('Alis', brow, .011, cel('hair')))
    parts.append(shapes.ball('Hidung', (0, dims.FACE_Y - .012, Z - .06), (.03, .028, .024), cel('skin_dark')))
    grin = [(x, dims.FACE_Y + .004 + abs(x) * .22, z) for x, _, z in
            shapes.arc((.012, 0, Z - .085), .085, 205, 335)]
    parts.append(shapes.tube('Senyum', grin, .009, cel('ink')))
    return parts


def hair():
    """A cap of hair plus tufts that lean one way, as if he has been pushing the cart all morning."""
    parts = [shapes.ball('Rambut', (0, .035, Z + .075), (.345, .315, .27), cel('hair'), 32, True)]
    tufts = [(-.25, -.1, .13, 58, .06), (-.15, -.2, .2, 30, .07), (-.02, -.235, .235, 12, .075),
             (.12, -.215, .22, -8, .07), (.24, -.12, .15, -42, .06), (.03, -.05, .3, 20, .07)]
    for x, y, z, lean, size in tufts:
        tilt = math.radians(lean)
        tip = (x - math.sin(tilt) * size * 1.1, y - .035, Z + z + math.cos(tilt) * size * 1.1)
        parts.append(shapes.capsule('Jambul', (x, y, Z + z - .04), tip, size, size * .5, cel('hair'), 14))
    for x in (-.2, -.07, .07, .2):   # the nape: uneven ends, he cuts it himself
        root, tip = (x, .24, Z + .0), (x * 1.1, .3, Z - .075 + abs(x) * .1)
        parts.append(shapes.capsule('Tengkuk', root, tip, .09, .06, cel('hair'), 14))
    return parts
