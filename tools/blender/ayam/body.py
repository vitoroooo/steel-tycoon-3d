"""The torso: breast, back and rump under a coat of saddle feathers, and the folded wings."""
import math
from .feathers import egg, plume

BREAST, BREAST_SIZE = (0, .09, .245), (.15, .15, .16)


def torso():
    parts = [egg('Body', (.17, .31, .19), (0, 0, .25), 'feather_body', lumps=.035, taper=.25),
             egg('Breast', BREAST_SIZE, BREAST, 'feather_body', lumps=.03),
             egg('Rump', (.14, .17, .14), (0, -.14, .285), 'feather_body', lumps=.03)]
    # Saddle feathers spill off the back over the root of the tail.
    for i in range(11):
        side = (i - 5) / 5
        root = (side * .05, -.1, .335 - abs(side) * .02)
        tip = (side * .095, -.23, .27 - abs(side) * .035)
        parts.append(plume('Saddle', root, (side * .085, -.19, .345), tip, .024, (side, 0, 1), 'feather_neck'))
    return parts + bib()


def bib():
    """Two rows of contour feathers lapping down the breast, so it reads as plumage and not as a ball."""
    def on_breast(turn, lift, puff=1.0):
        ring = math.cos(lift) * puff
        return (BREAST[0] + math.sin(turn) * BREAST_SIZE[0] / 2 * ring, BREAST[1] + math.cos(turn) * BREAST_SIZE[1] / 2 * ring,
                BREAST[2] + math.sin(lift) * BREAST_SIZE[2] / 2 * puff)
    parts = []
    for row, lift in enumerate((.55, -.05)):
        for i in range(7):
            turn = (i - 3 + row * .5) * .42
            root, bend, tip = on_breast(turn, lift, .98), on_breast(turn, lift - .4, 1.14), on_breast(turn, lift - .8, 1.03)
            parts.append(plume('Bib', root, bend, tip, .03, (math.sin(turn), math.cos(turn), 0), 'feather_body'))
    return parts


def wing(side):
    """A folded wing: the shoulder mass, then the flight feathers lying back along the flank."""
    x = side * .088
    parts = [egg('Wing', (.04, .2, .12), (x, .0, .275), 'feather_wing', lumps=.02, taper=.3)]
    for i in range(6):
        drop = i * .017
        root, tip = (x + side * .008, .04 - i * .012, .3 - drop), (x + side * .004, -.19 + i * .012, .255 - drop * 1.5)
        bend = (x + side * .022, -.08, .305 - drop)
        parts.append(plume('Flight', root, bend, tip, .024, (side, 0, .25), 'feather_wing'))
    # Short coverts over the shoulder break the smooth outline.
    for i in range(5):
        angle = -.5 + i * .25
        root = (x + side * .014, .07 - i * .03, .315)
        tip = (x + side * .02, .07 - i * .03 - .06 * math.cos(angle), .262 - .02 * math.sin(angle))
        parts.append(plume('Covert', root, (x + side * .03, root[1] - .03, .3), tip, .02, (side, 0, .2), 'feather_body'))
    return parts
