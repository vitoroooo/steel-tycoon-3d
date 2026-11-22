"""Torso and limbs of a Sukabaja body, dressed by a Look. Each limb is its own solid so Unity can swing it."""
from .. import shapes
from ..paint import cel
from . import dims


def torso(look):
    shirt = cel(look.shirt)
    return [
        shapes.capsule('Badan', (0, 0, .47), (0, 0, .69), .2, .175, shirt, 28),
        shapes.capsule('Leher', (0, 0, .7), dims.NECK, .075, .07, cel('skin')),
        shapes.capsule('Celana', (0, 0, .4), (0, 0, .47), .2, .205, cel(look.trousers), 28),
        shapes.tube('Sabuk', shapes.arc((0, 0, .475), .207, 0, 360, 'xy', 17)[:-1], .014, cel('navy'), True),
    ]


def arm(side, look):
    sx, sy, sz = dims.SHOULDER
    ex, ey, ez = dims.ELBOW
    wx, wy, wz = dims.WRIST
    shoulder, elbow, wrist = (side * sx, sy, sz), (side * ex, ey, ez), (side * wx, wy, wz)
    cuff = shapes.arc((0, 0, 0), .078, 0, 360, 'xy', 13)[:-1]
    mid = [(side * (sx + (ex - sx) * .62) + x, sy + (ey - sy) * .62 + y, sz + (ez - sz) * .62) for x, y, _ in cuff]
    hand = (side * (wx + .012), wy - .03, wz - .07)
    bare = cel(look.shirt if look.long_sleeves else 'skin')
    parts = [
        shapes.capsule('Lengan baju', shoulder, (side * (sx + (ex - sx) * .62), sy + (ey - sy) * .62,
                                               sz + (ez - sz) * .62), .092, .08, cel(look.shirt)),
        shapes.tube('Lipatan', mid, .012, cel('cream'), True),
        shapes.capsule('Lengan atas', shoulder, elbow, .062, .058, bare),
        shapes.capsule('Lengan bawah', elbow, wrist, .058, .056, bare),
        shapes.ball('Sarung tangan', hand, (.078, .07, .085), cel(look.hands)),
        shapes.capsule('Jempol', hand, (hand[0] - side * .05, hand[1] - .055, hand[2] + .02), .036, .03, cel(look.hands), 12),
    ]
    if look.cuff:
        parts.append(shapes.tube('Manset', [(wrist[0] + x * .82, wrist[1] + y * .82, wrist[2] - .012) for x, y, _ in cuff],
                                 .016, cel(look.cuff), True))
    return parts


def leg(side, look):
    x = side * dims.HIP_X
    hem = shapes.arc((x, 0, dims.KNEE_Z + .045), .097, 0, 360, 'xy', 13)[:-1]
    shin = cel(look.trousers if look.long_legs else 'skin')
    parts = [
        shapes.capsule('Pipa celana', (x, 0, dims.HIP_Z), (side * .105, 0, dims.KNEE_Z + .05), .105, .095, cel(look.trousers)),
        shapes.tube('Kelim', hem, .012, cel('navy'), True),
        shapes.capsule('Betis', (side * .105, 0, dims.KNEE_Z + .06), (side * .105, .005, dims.ANKLE_Z), .066, .056, shin),
        shapes.ball('Kaki', (side * .11, -.045, .052), (.075, .125, .045), cel('skin' if look.strap else look.sandal)),
        shapes.block('Sandal', (side * .11, -.04, .016), (.17, .29, .032), cel(look.sandal), .45),
    ]
    if look.strap:   # closed shoes have none
        parts.append(shapes.tube('Tali sandal', [(side * .11 - .07, -.02, .03), (side * .11 - .035, -.075, .092),
                                    (side * .11, -.1, .1), (side * .11 + .035, -.075, .092),
                                    (side * .11 + .07, -.02, .03)], .014, cel(look.strap)))
    return parts
